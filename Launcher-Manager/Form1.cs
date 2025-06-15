using Launcher_Manager.Helper;
using System.Diagnostics;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace Launcher_Manager
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            refreshTimer = new System.Windows.Forms.Timer();
            refreshTimer.Interval = 5000;//5000; // 5000ms = 5 วินาที
            refreshTimer.Tick += async (s, args) => await RefreshDataAsync();
            refreshTimer.Start();

            await RefreshDataAsync(); // โหลดข้อมูลทันทีตอนเปิดฟอร์ม
        }

        private bool IsProcessRunning(string processName)
        {
            return Process.GetProcessesByName(processName).Any();
        }

        private HashSet<string> GetRunningDockerContainers()
        {
            var runningContainers = new HashSet<string>();

            var processInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c docker ps --format \"{{.Names}}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            using (var process = Process.Start(processInfo))
            {
                while (!process.StandardOutput.EndOfStream)
                {
                    string line = process.StandardOutput.ReadLine()?.Trim();
                    if (!string.IsNullOrEmpty(line))
                    {
                        runningContainers.Add(line);
                    }
                }
            }

            return runningContainers;
        }

        private void RefreshServerStatus_Docker()
        {
            var runningContainers = GetRunningDockerContainers();

            var expectedContainers = new Dictionary<string, string>
            {
                { "Login Server", "microservices-login-service-1" },
                { "Instance Server", "microservices-instance-service-1" },
                { "Char Server", "char-service" },
                { "Chat Server", "chat-service" },
                { "Inventory Server", "inventory-service" },
                { "Party Server", "party-service" },
                { "Guild Server", "guild-service" },
                { "Mongo DB", "microservices-mongo-1"}
            };

            ServerMoniter.Clear();

            foreach (var kvp in expectedContainers)
            {
                string serverName = kvp.Key;
                string containerName = kvp.Value;
                bool isRunning = runningContainers.Contains(containerName);

                ServerMoniter.SelectionColor = Color.Black;
                ServerMoniter.AppendText($"  {serverName} : ");

                ServerMoniter.SelectionColor = isRunning ? Color.Green : Color.Red;
                ServerMoniter.AppendText($"{(isRunning ? "Running" : "Not Running")}\n");
            }
        }

        //private void RefreshServerStatus_Docker2()
        //{
        //    var runningContainers = GetRunningDockerContainers();
        //
        //    var expectedContainers = new Dictionary<string, string>
        //    {
        //        { "Login Server", "microservices-login-service-1" },
        //        { "Instance Server", "microservices-instance-service-1" },
        //        { "Char Server", "char-service" },
        //        { "Chat Server", "chat-service" },
        //        { "Inventory Server", "inventory-service" },
        //        { "Party Server", "party-service" },
        //        { "Guild Server", "guild-service" },
        //        { "Mongo DB", "microservices-mongo-1" }
        //    };
        //
        //    ServerListView.BeginUpdate();
        //    ServerListView.Items.Clear();
        //
        //    foreach (var kvp in expectedContainers)
        //    {
        //        string serverName = kvp.Key;
        //        string containerName = kvp.Value;
        //        bool isRunning = runningContainers.Contains(containerName);
        //
        //        var item = new ListViewItem(serverName);
        //        item.SubItems.Add(isRunning ? "✅ Running" : "❌ Not Running");
        //        item.ForeColor = isRunning ? Color.Green : Color.Red;
        //
        //        ServerListView.Items.Add(item);
        //    }
        //
        //    ServerListView.EndUpdate();
        //}

        private void RefreshServerStatus()
        {
            var serverStatuses = new Dictionary<string, string>()
            {
                { "Login Server", IsProcessRunning("Login-Service") ? "Running" : "Not Running" },
                { "Instance Server", IsProcessRunning("Instance-Service") ? "Running" : "Not Running" },
                { "Char Server", IsProcessRunning("Char-Service") ? "Running" : "Not Running" },
                { "Chat Server", IsProcessRunning("Chat-Service") ? "Running" : "Not Running" },
                { "Inventory Server", IsProcessRunning("Chat-Service") ? "Running" : "Not Running" },
                { "Party Server", IsProcessRunning("Chat-Service") ? "Running" : "Not Running" },
                { "Guild Server", IsProcessRunning("Chat-Service") ? "Running" : "Not Running" }
            };

            ServerMoniter.Clear();

            foreach (var kvp in serverStatuses)
            {
                string serverName = kvp.Key;
                string status = kvp.Value;

                // เพิ่มชื่อเซิร์ฟเวอร์ (สีปกติ)
                ServerMoniter.SelectionColor = Color.Black;
                ServerMoniter.AppendText($"  {serverName} : ");

                // เพิ่มสถานะ พร้อมสีเขียวหรือแดง
                if (status == "Running")
                {
                    ServerMoniter.SelectionColor = Color.Green;
                }
                else
                {
                    ServerMoniter.SelectionColor = Color.Red;
                }
                ServerMoniter.AppendText($"{status}\n");
            }
        }

        private List<InstanceStatusResponse> _lastInstances = new List<InstanceStatusResponse>();
        private bool HasInstancesChanged(List<InstanceStatusResponse> newInstances)
        {
            // ถ้าจำนวนไม่เท่ากัน = เปลี่ยนแปลง
            if (newInstances.Count != _lastInstances.Count)
                return true;

            // ถ้า IDs ไม่ตรงกัน หรือข้อมูลไม่เหมือนกัน (เปรียบเทียบแบบง่ายๆ)
            foreach (var instance in newInstances)
            {
                var oldInstance = _lastInstances.FirstOrDefault(i => i.InstanceId == instance.InstanceId);
                if (oldInstance == null)
                    return true; // มี instance ใหม่

                // ถ้าอยากละเอียด เช็คฟิลด์อื่นๆ ว่าเปลี่ยนมั้ย
                if (oldInstance.PlayerCount != instance.PlayerCount
                    || oldInstance.Port != instance.Port
                    || oldInstance.InstanceName != instance.InstanceName
                    || oldInstance.StartedAt != instance.StartedAt)
                {
                    return true; // ข้อมูลเปลี่ยนแปลง
                }
            }

            // ถ้าเช็คครบไม่มีอะไรเปลี่ยนเลย
            return false;
        }

        private async Task RefreshDataAsync()
        {
            //LogMessage("INFO", "Refreshing data...");
            RefreshServerStatus_Docker();
            try
            {
                var instances = await DB.DBHelper.GetAllInstancesAsync();

                if (HasInstancesChanged(instances))
                {
                    LogMessage("INFO", "Refreshing data...");

                    _lastInstances = instances; // อัพเดต cache

                    dataGridView1.Invoke((MethodInvoker)delegate
                    {
                        dataGridView1.Rows.Clear();

                        foreach (var instance in instances)
                        {
                            var usage = Helper.Helper.GetUsageFromPid(instance.ProcessId);

                            dataGridView1.Rows.Add(
                                instance.InstanceId,
                                instance.InstanceName,
                                instance.Port,
                                instance.PlayerCount,
                                usage?.cpuTimeMs.ToString("0.0") + " ms" ?? "-",
                                usage?.ramMb.ToString("0.0") + " mb" ?? "-",
                                instance.StartedAt
                            );
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                LogMessage("ERROR", ex.Message);
            }
        }


        private void LogMessage(string type, string message)
        {
            Color? typeColor = null;
            string typeLabel = "";
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            switch (type.ToUpper())
            {
                case "INFO":
                    typeColor = Color.Green;
                    typeLabel = "[INFO]";
                    break;
                case "ERROR":
                    typeColor = Color.Red;
                    typeLabel = "[ERROR]";
                    break;
                case "WARN":
                case "WARNING":
                    typeColor = Color.Orange;
                    typeLabel = "[WARN]";
                    break;
                default:
                    // No label or color for other types
                    break;
            }

            richTextBox1.Invoke((MethodInvoker)delegate
            {
                richTextBox1.SelectionStart = richTextBox1.TextLength;
                richTextBox1.SelectionLength = 0;

                // Append timestamp (default color)
                richTextBox1.SelectionColor = richTextBox1.ForeColor;
                richTextBox1.AppendText($"{timestamp} ");

                // Append [TYPE] (colored only if known type)
                if (typeColor.HasValue && !string.IsNullOrEmpty(typeLabel))
                {
                    richTextBox1.SelectionColor = typeColor.Value;
                    richTextBox1.AppendText($"{typeLabel} ");
                }

                // Append message (default color)
                richTextBox1.SelectionColor = richTextBox1.ForeColor;
                richTextBox1.AppendText($"{message}\n");

                richTextBox1.ScrollToCaret();
            });
        }

        private async void ShutdownServer()
        {
            using var client = new HttpClient();
            var response = await client.PostAsync("http://localhost:5251/instance-service/shutdown", null);
            if (response.IsSuccessStatusCode)
            {
                //MessageBox.Show("Instance-Service has been killed.");
                LogMessage("INFO", "Instance-Service has been killed");
            }
            else
            {
                MessageBox.Show("Failed to send shutdown command.");
            }
        }

        private void StopDocker()
        {
            LogMessage("INFO", "Stopping services with Docker Compose...");

            var processInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c docker-compose down",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = @"A:\MMORPG\Microservices" // ที่อยู่ของ docker-compose.yml
            };

            using (var process = new Process())
            {
                process.StartInfo = processInfo;
                process.OutputDataReceived += (s, ev) => { if (ev.Data != null) LogMessage("DOCKER", ev.Data); };
                process.ErrorDataReceived += (s, ev) => { if (ev.Data != null) LogMessage("ERROR", ev.Data); };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
            }

            LogMessage("INFO", "Docker Compose stopped successfully.");
        }

        private async void KillServer_ClickAsync(object sender, EventArgs e)
        {
            try
            {
                StopDocker();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error killing server: " + ex.Message);
            }
        }

        private void StartDocker()
        {
            LogMessage("INFO", "Starting services with Docker Compose...");

            var processInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c docker-compose up -d",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = @"A:\MMORPG\Microservices" // ตำแหน่งที่มี docker-compose.yml
            };

            using (var process = new Process())
            {
                process.StartInfo = processInfo;
                process.OutputDataReceived += (s, ev) => { if (ev.Data != null) LogMessage("DOCKER", ev.Data); };
                process.ErrorDataReceived += (s, ev) => { if (ev.Data != null) LogMessage("ERROR", ev.Data); };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
            }

            LogMessage("INFO", "Docker Compose started successfully.");
        }

        private void StartServer_Default()
        {

            string exePath = @"A:\MMORPG\Microservices\Instance-Service\bin\Debug\net7.0\Instance-Service.exe";

            LogMessage("INFO", "Instance - Server Is Started");
            var processInfo = new ProcessStartInfo
            {
                FileName = exePath,
                UseShellExecute = true,
                CreateNoWindow = false,
                WorkingDirectory = Path.GetDirectoryName(exePath)
            };

            Process.Start(processInfo);
        }



        private void StartServer_Click(object sender, EventArgs e)
        {
            try
            {

                StartDocker();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error starting server: " + ex.Message);
            }
        }
    }
}