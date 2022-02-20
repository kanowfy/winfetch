using System.Diagnostics;
using System.Text.RegularExpressions;
using Pastel;

public class StatsGenerator
{
	public string? UserName { get; set; }
	public string? MachineName { get; set; }
	public int TotalNameLength { get; set; }
	public string? OS { get; set; }
	public string? Kernel { get; set; }
	public TimeSpan Uptime { get; set; }
	public long AvailableMemory { get; set; }
	public long TotalMemory { get; set; }
	public string? CPU { get; set; }
	public string? GPU { get; set; }

	public async Task ProcessStats()
	{
		SetHostName();
		await SetOSValues();
		await SetHardwareValues();
	}

	public async Task<string> SpawnProcess(string arguments)
	{
		Process p = new Process();
		p.StartInfo.FileName = "cmd.exe";
		p.StartInfo.Arguments = arguments;
		p.StartInfo.RedirectStandardOutput = true;
		p.Start();
		var streamReader = p.StandardOutput;
		string output = await streamReader.ReadToEndAsync();
		streamReader.Close();
		return output;
	}

	public async Task<string[]> ReadOSInfo()
	{
		string osArguments = "/c wmic os get caption,FreePhysicalMemory,LastBootUpTime,TotalVisibleMemorySize,version";
		string osData = await SpawnProcess(osArguments);

		var dataList = Regex.Split(osData, @"\s{2,}").ToList();
		dataList.RemoveRange(0, 5);
		return dataList.ToArray();
	}


	public async Task<string> ReadCPUInfo()
	{
		string cpuArguments = "/c wmic cpu get name";
		string cpuData = await SpawnProcess(cpuArguments);
		return Regex.Split(cpuData, @"\s{2,}")[1]
				  .Replace("(R)", "")
				  .Replace("Core(TM) ", "")
				  .Replace("CPU ", "");
	}

	public async Task<string> ReadGPUInfo()
	{
		string gpuArguments = "/c wmic path Win32_VideoController get name";
		string gpuData = await SpawnProcess(gpuArguments);
		return Regex.Split(gpuData, @"\s{2,}")[1];
	}

	public async Task SetHardwareValues()
	{
		CPU = await ReadCPUInfo();
		GPU = await ReadGPUInfo();
	}

	public void SetHostName()
	{
		UserName = Environment.UserName;
		MachineName = Environment.MachineName;
		TotalNameLength = UserName.Length + MachineName.Length + 1;
	}

	public async Task SetOSValues()
	{
		string[] OSData = await ReadOSInfo();
		OS = OSData[0].Replace("Microsoft ", "");
		long freeMemory = Int64.Parse(OSData[1]);
		string lastBoot = OSData[2];
		TotalMemory = Int64.Parse(OSData[3]);
		Kernel = OSData[4];

		AvailableMemory = TotalMemory - freeMemory;
		Uptime = CalculateUpTime(lastBoot);
	}


	public TimeSpan CalculateUpTime(string bootTime)
	{
		DateTime boot = DateTime.ParseExact(bootTime.Substring(8, 6), "HHmmss", 
									System.Globalization.CultureInfo.InvariantCulture);
		return DateTime.Now - boot;
	}

	public string UptimeDisplay()
	{
		var sb = new System.Text.StringBuilder();
		if (Uptime.Days > 0)
			sb.AppendFormat($"{Uptime.Days}d ");
		sb.AppendFormat($"{Uptime.Hours}h ");
		sb.AppendFormat($"{Uptime.Minutes}m");
		return sb.ToString();
	}

	public void PrintStat(string[] logo)
	{
		
		for (int i = 0; i < logo.Length; i++)
		{
			if (i == 0) Console.WriteLine(logo[i] + "   " + UserName.Pastel(Color.Green) + '@' + MachineName.Pastel(Color.Red));
        		else if (i == 1) Console.WriteLine(logo[i] + "   " + string.Concat(Enumerable.Repeat("─", TotalNameLength)));
			else if (i == 2) Console.WriteLine(logo[i] + "   ■ os".Pastel(Color.Yellow) + $"      {OS}");
			else if (i == 3) Console.WriteLine(logo[i] + "   ■ kernel".Pastel(Color.Yellow) + $"  {Kernel}");
			else if (i == 4) Console.WriteLine(logo[i] + "   ■ uptime".Pastel(Color.Yellow) + $"  {UptimeDisplay()}");
			else if (i == 5) Console.WriteLine(logo[i] + "   ■ cpu".Pastel(Color.Yellow) + $"     {CPU}");
			else if (i == 6) Console.WriteLine(logo[i] + "   ■ gpu".Pastel(Color.Yellow) + $"     {GPU}");
			else if (i == 7) Console.WriteLine(logo[i] + "   ■ memory".Pastel(Color.Yellow) + $"  {AvailableMemory / 1024}mb / {TotalMemory / 1024}mb");
			else Console.WriteLine(logo[i]);
		}
	}
}