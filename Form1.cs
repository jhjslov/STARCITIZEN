using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Gma.System.MouseKeyHook;
using System.IO;
using System.Diagnostics;
using System.Drawing.Imaging; //PNG 저장관련

namespace SC_StepByStep_v1
{
    public partial class Form1 : Form
    {
        // ============================================================
        // [SECTION 1. API 및 상수 선언]
        // ============================================================
        [DllImport("user32.dll")] static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, int dwExtraInfo);
        [DllImport("user32.dll")] static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        [DllImport("user32.dll")] static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")] static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")] static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")] static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_RESTORE = 9; // 최소화된 경우 복원
        private const uint MOUSEEVENTF_MOVE = 0x0001;
        private const int KEYEVENTF_SCANCODE = 0x0008;
        private const int KEYEVENTF_KEYUP = 0x0002;

        // ============================================================
        // [SECTION 2. 전역 변수 및 상태 관리]
        // ============================================================
        private IKeyboardMouseEvents _globalHook;
        private System.Windows.Forms.Timer _syncTimer;
        private TcpListener _listener;
        private Stopwatch _cycleWatch = new Stopwatch();

        private bool _isMasterRole = true;      // true: 메인, false: 서브
        private bool _isSyncEnabled = false;    // F12 동기화 여부
        private bool _isMacroRunning = false;   // F3 매크로 실행 여부
        private bool _isSubReady = false;       // 서브 PC 응답 대기 상태
        private System.Windows.Forms.Timer _antiKickTimer; // 안티 킥 타이머

        private Point _lastPos;
        private int _deltaX = 0, _deltaY = 0;
        private string _configPath = AppDomain.CurrentDomain.BaseDirectory + "config.txt";

        // 화물 버튼 관리를 위한 구조체
        private struct CargoButton { public Point Pos; public Bitmap Image; public string Name; }
        private CargoButton _btnDown, _btnConfirm, _btnUp;

        private string _savedMainIP = "";
        private string _savedSubIP = "";
        private int _captureIndex = 0; // 0:Down, 1:Confirm, 2:Up 우클릭 순환용
        private int _editTarget = -1; // -1: 순차모드, 0:Down, 1:Confirm, 2:Up 개별수정모드
        private bool _waitingForUserClick = false; // 서브 수동 모드용 플래그
        private string _imgDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images"); // 이미지 폴더 경로

        // ============================================================
        // [SECTION 3. 초기화 및 생성자]
        // ============================================================
        public Form1()
        {
            InitializeComponent();

            // 1. 기본 설정
            this.TopMost = true;
            this.Load += (s, e) => InitializeSystem();

            // 2. 이벤트 핸들러 명시적 연결 (여기에 모아두면 한눈에 보기 편합니다)
            // 디자인 창에서 연결을 지웠으므로 여기서 딱 한 번씩만 연결됩니다.
            btnRoleMaster.Click += btnRoleMaster_Click;
            btnRoleSlave.Click += btnRoleSlave_Click;
            btnConnTest.Click += btnConnTest_Click;
            btnSaveConfig.Click += btnSaveConfig_Click;

            // 체크박스들
            chkAlwaysOnTop.CheckedChanged += (s, e) => { this.TopMost = chkAlwaysOnTop.Checked; };
            // chkCaptureEnable은 굳이 연결 안 해도 코드 내에서 .Checked로 읽어오므로 생략 가능
        }

        private void InitializeSystem()
        {
            lblMyIP.Text = GetLocalIP();

            // 1. 설정 로드

            lstLog.Font = new Font("돋움", 8);  // [수정] 로그 폰트 크기 조정 (8pt)
            _antiKickTimer = new System.Windows.Forms.Timer { Interval = 300000 }; // [추가] 안티 킥 타이머 설정 (5분 = 300,000ms)
            _antiKickTimer.Tick += (s, e) =>
            {
                if (chkAntiKick.Checked)
                {
                    mouse_event(MOUSEEVENTF_MOVE, 5, 0, 0, 0);  // 우로 ５px
                    System.Threading.Thread.Sleep(50);
                    mouse_event(MOUSEEVENTF_MOVE, -5, 0, 0, 0); // 좌로 ５px
                    UpdateLog("🛡️ 안티 킥: 마우스 미세 이동 수행");
                }
            };
            _antiKickTimer.Start();

            LoadSettings();

            // 2. 초기 모드 설정 및 UI 반영
            _isMasterRole = true;
            UpdateRoleUI();

            // 3. 서비스 시작
            InitPictureBoxEvents();
            Subscribe(); // 후킹 시작
            _lastPos = Cursor.Position;
            StartServer(); // TCP 서버 대기
            InitSyncTimer(); // 마우스 동기화 타이머

            _cycleWatch.Start();
            UpdateLog("🚀 시스템 v3.6.0 활성화 (Master Mode)");
        }
        private void InitPictureBoxEvents()
        {
            // 이미지 박스를 클릭하면 "준비 상태"가 됩니다.
            picDown.Click += (s, e) =>
            {
                _editTarget = 0;
                UpdateLog("🎯 [Down] 수정 대기 중... 게임 화면 위에서 우클릭하세요.");
            };
            picConfirm.Click += (s, e) =>
            {
                _editTarget = 1;
                UpdateLog("🎯 [Confirm] 수정 대기 중... 게임 화면 위에서 우클릭하세요.");
            };
            picUp.Click += (s, e) =>
            {
                _editTarget = 2;
                UpdateLog("🎯 [Up] 수정 대기 중... 게임 화면 위에서 우클릭하세요.");
            };
        }

        // ============================================================
        // [SECTION 4. 핵심 자동화 로직 (Cargo Macro)]
        // ============================================================
        private async void StartCargoAutomation()
        {
            if (_btnDown.Image == null || _btnConfirm.Image == null || _btnUp.Image == null)
            {
                UpdateLog("❌ 오류: 버튼 3종 이미지가 없습니다.");
                return;
            }

            try
            {
                _isMacroRunning = true;
                int totalRepeat = GetInt(txtRepeat, 1); // 반복 횟수 읽기
                UpdateLog($"▶ 매크로 시작 (총 {totalRepeat}회 진행)");

                for (int i = 1; i <= totalRepeat; i++)
                {
                    // [체크] 매크로 중지 여부 확인
                    if (!_isMacroRunning) break;

                    UpdateLog($"--- {i} / {totalRepeat} 회차 진행 중 ---");

                    // 1. CargoDown 단계
                    if (!await WaitAndClick(_btnDown, "Down", GetInt(txtWaitMatch, 5000))) break;

                    // 2. Confirm 인식 단계
                    await Task.Delay(GetInt(txtWaitConfirm, 1000));
                    Cursor.Position = _btnConfirm.Pos;
                    Stopwatch sw = Stopwatch.StartNew();
                    int matchTimeout = GetInt(txtWaitMatch, 5000);

                    bool isMatched = false;
                    while (sw.ElapsedMilliseconds < matchTimeout)
                    {
                        if (!_isMacroRunning) return;
                        // [수정] CheckMatch 대신 SearchImageInRange를 사용하여 30% 영역 스캔 적용
                        if (SearchImageInRange(_btnConfirm)) { isMatched = true; break; }
                        await Task.Delay(200);
                    }

                    if (!isMatched) { UpdateLog("❌ Confirm 인식 실패"); break; }

                    // 3. 서브 1차 동작 (클릭)
                    _isSubReady = false; // 신호 대기 초기화
                    SendCommand("SUB_ACTION_1");
                    await Task.Delay(GetInt(txtWaitClick, 500));
                    mouse_event(0x0002 | 0x0004, 0, 0, 0, 0);

                    // 4. CargoUp 단계
                    await Task.Delay(GetInt(txtWaitUp, 1000));
                    if (!await WaitAndClick(_btnUp, "Up", GetInt(txtWaitMatch, 5000))) break;

                    // 5. ESC 및 서브 2차 동작 (F키)
                    await Task.Delay(1000);
                    SendKeyScan(0x01, 500); // ESC 입력

                    _isSubReady = false; // 신호 대기 초기화
                    SendCommand("SUB_ACTION_2");

                    // [중요] 서브 PC 응답 대기 (최대 10초 타임아웃 추가)
                    Stopwatch subWaitSw = Stopwatch.StartNew();
                    while (!_isSubReady)
                    {
                        if (!_isMacroRunning || subWaitSw.ElapsedMilliseconds > 10000) break;
                        await Task.Delay(100);
                    }

                    if (_isSubReady)
                    {
                        UpdateLog("✅ 서브 PC 응답 확인. 동작 안정화를 위해 대기...");

                        // [추가] 신호를 받은 후 실제 동작 전 대기 시간 적용
                        // txtWaitAction 텍스트박스의 값을 읽어옵니다 (기본값 500ms)
                        int actionWait = GetInt(txtWaitAction, 500);
                        await Task.Delay(actionWait);
                    }
                    else
                    {
                        UpdateLog("⚠️ 서브 PC 응답 지연(타임아웃).");
                    }

                    // 6. 마무리 동작 수행
                    UpdateLog("⌨️ 마무리 F 키 입력");
                    SendKeyScan(0x21); // F키 (ScanCode 0x21)
                    await Task.Delay(200);
                    SendKeyScan(0x21);

                    // 다음 회차 대기
                    int nextWait = GetInt(txtWaitNext, 2000);
                    UpdateLog($"회차 종료. {nextWait}ms 후 다음 회차 시작...");
                    await Task.Delay(nextWait);
                }
            }
            catch (Exception ex) { UpdateLog($"❌ 에러 발생: {ex.Message}"); }
            finally
            {
                _isMacroRunning = false;
                UpdateLog("🏁 모든 매크로 시퀀스 종료");
                // [수정] 체크박스가 켜져있을 때만 종료 알림음
                if (chkSoundEnable.Checked) System.Media.SystemSounds.Asterisk.Play();
            }
        }

        // ============================================================
        // [SECTION 5. 네트워크 통신 (TCP Server/Client)]
        // ============================================================
        private async void StartServer()
        {
            try
            {
                _listener = new TcpListener(IPAddress.Any, 8888);
                _listener.Start();
                while (true)
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync();
                    _ = Task.Run(async () =>
                    {
                        using (client) using (NetworkStream stream = client.GetStream())
                        {
                            byte[] buffer = new byte[1024];
                            int len = await stream.ReadAsync(buffer, 0, buffer.Length);
                            if (len > 0)
                            {
                                string msg = Encoding.UTF8.GetString(buffer, 0, len);
                                this.BeginInvoke(new Action(() => ProcessCommand(msg)));
                            }
                        }
                    });
                }
            }
            catch { }
        }
        private void SendCommand(string msg)
        {
            string fullMsg = $"{msg}|{GetLocalIP()}";
            Task.Run(() =>
            {
                try
                {
                    using (TcpClient client = new TcpClient())
                    {
                        if (client.ConnectAsync(txtTargetIP.Text, 8888).Wait(500))
                        {
                            byte[] data = Encoding.UTF8.GetBytes(fullMsg);
                            client.GetStream().Write(data, 0, data.Length);
                        }
                    }
                }
                catch { }
            });
        }
        private void ProcessCommand(string cmd)
        {
            // 수신 신호 시각적 알림 (라벨 깜빡임)
            FlashStatusLabel();

            string[] p = cmd.Split('|');
            string commandName = p[0];
            string senderIP = (p.Length > 1) ? p[1] : txtTargetIP.Text;

            if (_isMasterRole)
            {
                // ==========================================
                // [메인 PC] 서브 PC로부터의 응답 처리
                // ==========================================
                if (commandName == "SUB_READY")
                {
                    _isSubReady = true;
                    UpdateLog("✅ 서브 PC로부터 응답 수신 (다음 단계 진행 가능)");
                }
            }
            else
            {
                // ==========================================
                // [서브 PC] 메인 PC로부터의 명령 처리
                // ==========================================

                // 메인 PC IP 자동 업데이트 (통신 편의성)
                if (senderIP != "127.0.0.1" && txtTargetIP.Text != senderIP)
                {
                    txtTargetIP.Text = senderIP;
                }

                // --- 상황 A: 서브 수동 모드 (사용자 클릭 대기) ---
                if (chkSubManual.Checked)
                {
                    if (commandName == "SUB_ACTION_1" || commandName == "SUB_ACTION_2")
                    {
                        _waitingForUserClick = true;
                        UpdateLog($"📥 [수동 모드] 메인 신호({commandName}) 수신. 화면을 직접 클릭하세요!");
                        return; // 아래의 자동 실행 로직을 타지 않도록 종료
                    }
                }

                // --- 상황 B: 서브 자동 모드 (기존 매크로 실행) ---
                if (commandName == "SUB_ACTION_1")
                {
                    Task.Run(async () =>
                    {
                        // 1. 첫 번째 클릭 수행
                        mouse_event(0x0002 | 0x0004, 0, 0, 0, 0);

                        // 2. 디싱크 보정: 마우스 하강
                        if (chkSyncMove.Checked)
                        {
                            int dist = GetInt(txtSyncDist, 100);
                            await Task.Delay(100);
                            mouse_event(0x0001, 0, dist, 0, 0);
                            UpdateLog($"↕️ 보정: 하강 {dist}px");
                        }
                        SendCommand("SUB_READY");
                    });
                }
                else if (commandName == "SUB_ACTION_2")
                {
                    Task.Run(async () =>
                    {
                        try
                        {
                            // 1. 게임창(StarCitizen) 찾기 및 활성화
                            Process[] processes = Process.GetProcessesByName("StarCitizen");
                            if (processes.Length > 0)
                            {
                                IntPtr hWnd = processes[0].MainWindowHandle;
                                if (hWnd != IntPtr.Zero)
                                {
                                    SetForegroundWindow(hWnd);
                                    await Task.Delay(200);
                                    mouse_event(0x0002, 0, 0, 0, 0); // 포커스용 클릭
                                    await Task.Delay(50);
                                    mouse_event(0x0004, 0, 0, 0, 0);
                                    await Task.Delay(200);
                                }
                            }
                        }
                        catch { }

                        int waitTime = GetInt(txtWaitEsc, 1000);
                        await Task.Delay(waitTime);

                        // 2. 두 번째 클릭 수행
                        mouse_event(0x0002, 0, 0, 0, 0);
                        await Task.Delay(150);
                        mouse_event(0x0004, 0, 0, 0, 0);

                        // 3. 디싱크 보정: 마우스 상승
                        if (chkSyncMove.Checked)
                        {
                            int dist = GetInt(txtSyncDist, 100);
                            await Task.Delay(100);
                            mouse_event(0x0001, 0, -dist, 0, 0);
                            UpdateLog($"↕️ 보정: 상승 {dist}px");
                        }

                        UpdateLog("🖱️ 서브 액션 2 완료");
                        SendCommand("SUB_READY");
                    });
                }
                // --- 기타 동기화 명령 ---
                else if (commandName == "M_MOVE") mouse_event(0x0001, int.Parse(p[1]), int.Parse(p[2]), 0, 0);
                else if (commandName == "K_DOWN") ExecuteKey(p[1], false);
                else if (commandName == "K_UP") ExecuteKey(p[1], true);
            }
        }

        // ============================================================
        // [SECTION 6. UI 이벤트 및 버튼 제어]
        // ============================================================
        private void btnRoleMaster_Click(object sender, EventArgs e)
        {
            _isMasterRole = true;
            UpdateRoleUI();
        }
        private void btnRoleSlave_Click(object sender, EventArgs e)
        {
            _isMasterRole = false;
            UpdateRoleUI();
        }
        private void btnConnTest_Click(object sender, EventArgs e)
        {
            string ip = txtTargetIP.Text;
            UpdateLog($"🔍 {ip} 연결 테스트...");
            Task.Run(() =>
            {
                try
                {
                    using (TcpClient client = new TcpClient())
                    {
                        var res = client.BeginConnect(ip, 8888, null, null);
                        if (res.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1)))
                        {
                            client.EndConnect(res);
                            UpdateLog("✅ 통신 가능");
                        }
                        else UpdateLog("❌ 통신 불가 (포트/IP 확인)");
                    }
                }
                catch (Exception ex) { UpdateLog("⚠️ 오류: " + ex.Message); }
            });
        }
        private void btnSaveConfig_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. IP 및 기본 설정 저장
                if (_isMasterRole) { _savedMainIP = GetLocalIP(); _savedSubIP = txtTargetIP.Text; }
                else { _savedSubIP = GetLocalIP(); _savedMainIP = txtTargetIP.Text; }

                string[] lines = {
            _savedMainIP, _savedSubIP, txtRatioX.Text, txtRatioY.Text, txtRepeat.Text,
            chkAlwaysOnTop.Checked.ToString(), txtWaitMatch.Text, txtWaitAction.Text,
            txtWaitNext.Text, chkCaptureEnable.Checked.ToString(),
            chkSoundEnable.Checked.ToString(), chkAntiKick.Checked.ToString(),
            chkSyncMove.Checked.ToString(), txtSyncDist.Text,
            $"{_btnDown.Pos.X},{_btnDown.Pos.Y}", // 14번 라인: Down 좌표
            $"{_btnConfirm.Pos.X},{_btnConfirm.Pos.Y}", // 15번 라인: Confirm 좌표
            $"{_btnUp.Pos.X},{_btnUp.Pos.Y}" // 16번 라인: Up 좌표
        };
                File.WriteAllLines(_configPath, lines);

                // 2. PNG 형식으로 Images 폴더에 저장
                if (picDown.Image != null) picDown.Image.Save(Path.Combine(_imgDir, "cap_down.png"), System.Drawing.Imaging.ImageFormat.Png);
                if (picConfirm.Image != null) picConfirm.Image.Save(Path.Combine(_imgDir, "cap_confirm.png"), System.Drawing.Imaging.ImageFormat.Png);
                if (picUp.Image != null) picUp.Image.Save(Path.Combine(_imgDir, "cap_up.png"), System.Drawing.Imaging.ImageFormat.Png);

                UpdateLog("💾 설정 및 PNG 이미지 저장 완료 (Images 폴더)");
            }
            catch (Exception ex) { UpdateLog($"❌ 저장 실패: {ex.Message}"); }
        }

        private void UpdateRoleUI()
        {
            btnRoleMaster.BackColor = _isMasterRole ? Color.LightGreen : SystemColors.Control;
            btnRoleSlave.BackColor = !_isMasterRole ? Color.LightSalmon : SystemColors.Control;

            // 저장된 값이 있다면 해당 값을, 없다면 기본 가이드 텍스트를 출력
            if (_isMasterRole)
            {
                txtTargetIP.Text = string.IsNullOrEmpty(_savedSubIP) ? "서브 PC IP 입력" : _savedSubIP;
            }
            else
            {
                txtTargetIP.Text = string.IsNullOrEmpty(_savedMainIP) ? "메인 PC IP 입력" : _savedMainIP;
            }
        }

        // ============================================================
        // [SECTION 7. 마우스/키보드 유틸리티]
        // ============================================================
        private void Subscribe()
        {
            _globalHook = Hook.GlobalEvents();
            _globalHook.MouseMove += (s, e) =>
            {
                if (!_isSyncEnabled || !_isMasterRole) { _lastPos = e.Location; return; }
                _deltaX += e.X - _lastPos.X; _deltaY += e.Y - _lastPos.Y;
                _lastPos = e.Location;
            };

            _globalHook.MouseDown += async (s, e) =>
            {
                // ==========================================
                // 1. [서브 PC 전용] 수동 모드 클릭 감지
                // ==========================================
                if (!_isMasterRole && chkSubManual.Checked)
                {
                    // 메인으로부터 신호를 받아 대기 중일 때 좌클릭하면 신호 전송
                    if (e.Button == MouseButtons.Left && _waitingForUserClick)
                    {
                        _waitingForUserClick = false; // 대기 상태 해제
                        UpdateLog("🖱️ [수동] 사용자 클릭 감지 -> 메인에 완료 신호 전송");
                        SendCommand("SUB_READY");
                        return; // 수동 클릭 처리 완료 후 종료
                    }
                }

                // ==========================================
                // 2. [메인 PC 전용] 버튼 캡처 및 수정 로직 (우클릭)
                // ==========================================
                if (_isMasterRole && e.Button == MouseButtons.Right)
                {
                    Point fixedPos = e.Location;

                    // --- 상황 A: 개별 수정 모드 (체크박스 여부와 상관없이 작동) ---
                    if (_editTarget != -1)
                    {
                        UpdateLog($"📸 [개별 캡처] 시작 (좌표: {fixedPos.X}, {fixedPos.Y})");
                        Bitmap avgCap = await CaptureAverageImage(fixedPos);

                        if (_editTarget == 0)
                        {
                            _btnDown = new CargoButton { Pos = fixedPos, Image = avgCap, Name = "Down" };
                            picDown.Image = (Bitmap)avgCap.Clone();
                        }
                        else if (_editTarget == 1)
                        {
                            _btnConfirm = new CargoButton { Pos = fixedPos, Image = avgCap, Name = "Confirm" };
                            picConfirm.Image = (Bitmap)avgCap.Clone();
                        }
                        else if (_editTarget == 2)
                        {
                            _btnUp = new CargoButton { Pos = fixedPos, Image = avgCap, Name = "Up" };
                            picUp.Image = (Bitmap)avgCap.Clone();
                        }

                        _editTarget = -1; // 수정 모드 초기화
                        UpdateLog("✅ 개별 버튼 수정 완료 (저장 버튼을 눌러 확정하세요)");
                        return;
                    }

                    // --- 상황 B: 일반 순차 캡처 모드 (체크박스 켜져 있을 때만) ---
                    if (chkCaptureEnable.Checked)
                    {
                        UpdateLog($"📸 순차 캡처 진행 중... ({_captureIndex + 1}/3)");
                        Bitmap cap = await CaptureAverageImage(fixedPos);

                        if (_captureIndex == 0)
                        {
                            _btnDown = new CargoButton { Pos = fixedPos, Image = cap, Name = "Down" };
                            picDown.Image = (Bitmap)cap.Clone();
                            _captureIndex = 1;
                            UpdateLog("📸 1/3: Down 저장 완료");
                        }
                        else if (_captureIndex == 1)
                        {
                            _btnConfirm = new CargoButton { Pos = fixedPos, Image = cap, Name = "Confirm" };
                            picConfirm.Image = (Bitmap)cap.Clone();
                            _captureIndex = 2;
                            UpdateLog("📸 2/3: Confirm 저장 완료");
                        }
                        else if (_captureIndex == 2)
                        {
                            _btnUp = new CargoButton { Pos = fixedPos, Image = cap, Name = "Up" };
                            picUp.Image = (Bitmap)cap.Clone();
                            _captureIndex = 0; // 순환 완료
                            UpdateLog("📸 3/3: Up 저장 완료 (순차 캡처 종료)");
                        }
                    }
                }
            };

            _globalHook.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.F12)
                {
                    _isSyncEnabled = !_isSyncEnabled;
                    UpdateLog(_isSyncEnabled ? "▶ 동기화ON" : "■ 동기화OFF");
                }
                else if (e.KeyCode == Keys.F3 && _isMasterRole)
                {
                    if (!_isMacroRunning)
                    {
                        StartCargoAutomation();
                    }
                    else
                    {
                        _isMacroRunning = false;
                        UpdateLog("🛑 매크로 중단 요청됨..."); // 중단 신호 즉시 반영
                    }
                }
                else if (_isSyncEnabled && _isMasterRole && IsTargetKey(e.KeyCode)) SendCommand($"K_DOWN|{e.KeyCode}");
            };

            _globalHook.KeyUp += (s, e) =>
            {
                if (_isSyncEnabled && _isMasterRole && IsTargetKey(e.KeyCode)) SendCommand($"K_UP|{e.KeyCode}");
            };
        }
        private void InitSyncTimer()
        {
            _syncTimer = new System.Windows.Forms.Timer { Interval = 10 };
            _syncTimer.Tick += (s, e) =>
            {
                if (_isSyncEnabled && _isMasterRole && (_deltaX != 0 || _deltaY != 0))
                {
                    double.TryParse(txtRatioX.Text, out double rX); double.TryParse(txtRatioY.Text, out double rY);
                    SendCommand($"M_MOVE|{(int)(_deltaX * (rX == 0 ? 1 : rX))}|{(int)(_deltaY * (rY == 0 ? 1 : rY))}");
                    _deltaX = 0; _deltaY = 0;
                }
            };
            _syncTimer.Start();
        }

        // ============================================================
        // [SECTION 8. 시스템 유틸리티]
        // ============================================================
        private async Task<bool> WaitAndClick(CargoButton btn, string name, int timeout)
        {
            if (!_isMacroRunning) return false;

            Cursor.Position = btn.Pos; // 저장된 고정 좌표로 이동
            await Task.Delay(300);

            Stopwatch sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < timeout)
            {
                if (!_isMacroRunning) return false;

                // 단순히 한 지점만 보는 게 아니라 주변 영역(30%)을 검색
                if (SearchImageInRange(btn))
                {
                    mouse_event(0x0002 | 0x0004, 0, 0, 0, 0); // 찾으면 클릭
                    return true;
                }
                await Task.Delay(200);
            }
            return false;
        }
        private void SendKeyScan(byte sc, int delay = 100)
        {
            keybd_event(0, sc, KEYEVENTF_SCANCODE, 0);
            System.Threading.Thread.Sleep(delay);
            keybd_event(0, sc, KEYEVENTF_SCANCODE | KEYEVENTF_KEYUP, 0);
        }

        private void LoadSettings()
        {
            if (!File.Exists(_configPath)) return;
            try
            {
                string[] l = File.ReadAllLines(_configPath);
                if (l.Length >= 14)
                {
                    // 기존 텍스트 설정 로드 (0~13번 생략...)
                    _savedMainIP = l[0]; _savedSubIP = l[1];
                    txtRatioX.Text = l[2]; txtRatioY.Text = l[3]; txtRepeat.Text = l[4];
                    chkAlwaysOnTop.Checked = bool.Parse(l[5]);
                    txtWaitMatch.Text = l[6]; txtWaitAction.Text = l[7]; txtWaitNext.Text = l[8];
                    chkCaptureEnable.Checked = bool.Parse(l[9]);
                    chkSoundEnable.Checked = bool.Parse(l[10]);
                    chkAntiKick.Checked = bool.Parse(l[11]);
                    chkSyncMove.Checked = bool.Parse(l[12]);
                    txtSyncDist.Text = l[13];
                    this.TopMost = chkAlwaysOnTop.Checked;

                    // 3. 좌표 및 이미지 복구 (14~16번 라인)
                    if (l.Length >= 17)
                    {
                        _btnDown.Pos = StringToPoint(l[14]);
                        _btnConfirm.Pos = StringToPoint(l[15]);
                        _btnUp.Pos = StringToPoint(l[16]);

                        _btnDown.Image = LoadImageFile("cap_down.png", picDown);
                        _btnConfirm.Image = LoadImageFile("cap_confirm.png", picConfirm);
                        _btnUp.Image = LoadImageFile("cap_up.png", picUp);
                    }

                    UpdateLog("✅ 모든 설정 및 이미지 로드 완료");
                }
            }
            catch { UpdateLog("⚠️ 설정 로드 중 오류 발생"); }
        }

        // 문자열 "x,y"를 Point 객체로 변환하는 유틸리티
        private Point StringToPoint(string s)
        {
            string[] p = s.Split(',');
            return new Point(int.Parse(p[0]), int.Parse(p[1]));
        }

        // 파일에서 이미지를 안전하게 불러오는 유틸리티
        private Bitmap LoadImageFile(string fileName, PictureBox pb)
        {
            string path = Path.Combine(_imgDir, fileName);
            if (File.Exists(path))
            {
                using (var temp = new Bitmap(path))
                {
                    Bitmap bmp = new Bitmap(temp);
                    pb.Image = bmp;
                    return bmp;
                }
            }
            return null;
        }
        private string GetLocalIP()
        {
            try { using (Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0)) { s.Connect("8.8.8.8", 65530); return (s.LocalEndPoint as IPEndPoint).Address.ToString(); } }
            catch { return "127.0.0.1"; }
        }

        private bool IsTargetKey(Keys k) => k == Keys.W || k == Keys.A || k == Keys.S || k == Keys.D || k == Keys.F || k == Keys.LShiftKey;
        private void ExecuteKey(string keyStr, bool up) { if (Enum.TryParse(keyStr, out Keys k)) { byte sc = GetScanCode(k); keybd_event(0, sc, KEYEVENTF_SCANCODE | (up ? (uint)2 : 0), 0); } }
        private byte GetScanCode(Keys k) => k switch { Keys.W => 0x11, Keys.A => 0x1E, Keys.S => 0x1F, Keys.D => 0x20, Keys.F => 0x21, Keys.LShiftKey => 0x2A, _ => 0 };
        private int GetInt(TextBox t, int def) => int.TryParse(t.Text, out int r) ? r : def;
        private void UpdateLog(string m) { if (this.IsDisposed) return; this.BeginInvoke(new Action(() => { if (lstLog.Items.Count > 30) lstLog.Items.Clear(); lstLog.Items.Insert(0, $"[{DateTime.Now:HH:mm:ss}] {m}"); })); }
        private void FlashStatusLabel() { this.BeginInvoke(new Action(() => { lblStatus.BackColor = Color.Lime; Task.Delay(100).ContinueWith(_ => lblStatus.BackColor = Color.Transparent); })); }
        protected override void OnFormClosing(FormClosingEventArgs e) { _listener?.Stop(); _globalHook?.Dispose(); base.OnFormClosing(e); }
        // 1초 동안 '고정된 좌표'에서 10번 샘플링하여 평균 이미지 생성
        private async Task<Bitmap> CaptureAverageImage(Point fixedPos)
        {
            int sampleCount = 10;
            int width = 10, height = 10;
            long[,] rSum = new long[width, height], gSum = new long[width, height], bSum = new long[width, height];

            for (int i = 0; i < sampleCount; i++)
            {
                using (Bitmap bmp = new Bitmap(width, height))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                        // fixedPos를 기준으로 캡처 범위를 고정 (마우스 이동에 영향받지 않음)
                        g.CopyFromScreen(fixedPos.X - 5, fixedPos.Y - 5, 0, 0, new Size(width, height));

                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            Color c = bmp.GetPixel(x, y);
                            rSum[x, y] += c.R; gSum[x, y] += c.G; bSum[x, y] += c.B;
                        }
                    }
                }
                await Task.Delay(100);
            }

            Bitmap avgBmp = new Bitmap(width, height);
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    avgBmp.SetPixel(x, y, Color.FromArgb((int)(rSum[x, y] / sampleCount), (int)(gSum[x, y] / sampleCount), (int)(bSum[x, y] / sampleCount)));

            return avgBmp;
        }
        private bool SearchImageInRange(CargoButton btn)
        {
            int range = (int)(10 * 0.5); // 이미지 크기(10px)의 30%인 3px 반경 검색
            for (int ox = -range; ox <= range; ox++)
            {
                for (int oy = -range; oy <= range; oy++)
                {
                    Point searchPos = new Point(btn.Pos.X + ox, btn.Pos.Y + oy);
                    if (IsMatchAt(btn.Image, searchPos)) return true;
                }
            }
            return false;
        }
        private bool IsMatchAt(Bitmap target, Point p)
        {
            using (Bitmap cur = new Bitmap(10, 10))
            {
                using (Graphics g = Graphics.FromImage(cur))
                    g.CopyFromScreen(p.X - 5, p.Y - 5, 0, 0, new Size(10, 10));

                for (int x = 0; x < 10; x++)
                    for (int y = 0; y < 10; y++)
                    {
                        Color c1 = target.GetPixel(x, y); Color c2 = cur.GetPixel(x, y);
                        if (Math.Abs(c1.R - c2.R) > 25 || Math.Abs(c1.G - c2.G) > 25 || Math.Abs(c1.B - c2.B) > 25) return false;
                    }
                return true;
            }
        }
        private void SaveImages()
        {
            if (!Directory.Exists(_imgDir)) Directory.CreateDirectory(_imgDir);

            if (picDown.Image != null) picDown.Image.Save(Path.Combine(_imgDir, "cap_down.png"), ImageFormat.Png);
            if (picConfirm.Image != null) picConfirm.Image.Save(Path.Combine(_imgDir, "cap_confirm.png"), ImageFormat.Png);
            if (picUp.Image != null) picUp.Image.Save(Path.Combine(_imgDir, "cap_up.png"), ImageFormat.Png);
        }
        // 네트워크 메시지 수신 로직 (OnDataReceived) 예시
   

        private void chkSyncMove_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
