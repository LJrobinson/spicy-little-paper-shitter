using System.Text.Json;

var options = CliOptions.Parse(args);
var diagnostic = PrinterChaosEngine.Generate(options.RageLevel);

if (options.ShowHelp)
{
    HelpRenderer.Render();
    return;
}

if (options.Json)
{
    JsonRenderer.Render(diagnostic);
    return;
}

if (options.Ticket)
{
    TicketRenderer.Render(diagnostic);
    AchievementRenderer.TryRender(diagnostic);
    return;
}

if (options.Receipt)
{
    ReceiptRenderer.Render(diagnostic);
    AchievementRenderer.TryRender(diagnostic);
    return;
}

if (options.Fix)
{
    FixRenderer.Render(diagnostic);
    AchievementRenderer.TryRender(diagnostic);
    return;
}

DiagnosticRenderer.Render(diagnostic);
AchievementRenderer.TryRender(diagnostic);

public sealed class CliOptions
{
    public int RageLevel { get; private set; } = 5;
    public bool Json { get; private set; }
    public bool Ticket { get; private set; }
    public bool Receipt { get; private set; }
    public bool Fix { get; private set; }
    public bool ShowHelp { get; private set; }

    public static CliOptions Parse(string[] args)
    {
        var options = new CliOptions();

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--json":
                    options.Json = true;
                    break;

                case "--ticket":
                    options.Ticket = true;
                    break;

                case "--receipt":
                    options.Receipt = true;
                    break;

                case "--fix":
                    options.Fix = true;
                    break;

                case "--help":
                case "-h":
                    options.ShowHelp = true;
                    break;

                case "--rage":
                    if (i + 1 >= args.Length || !int.TryParse(args[i + 1], out var rage))
                    {
                        Console.WriteLine("Invalid --rage value. Using default rage level: 5");
                        break;
                    }

                    options.RageLevel = Math.Clamp(rage, 1, 10);
                    i++;
                    break;
            }
        }

        return options;
    }
}

public sealed record PrinterDiagnostic(
    string TicketId,
    string PrinterName,
    string Mood,
    string PaperStatus,
    string ConnectionStatus,
    string DriverStatus,
    string Diagnosis,
    string RecommendedFix,
    int ChaosRating
);

public sealed record FailureMode(
    string Code,
    string Diagnosis,
    string RecommendedFix,
    string InternalNote
);

public static class PrinterChaosEngine
{
    private static readonly Random Random = new();

    private static readonly string[] PrinterNames =
    [
        "Epson TM-T88V",
        "Star Micronics TSP100",
        "Bixolon SRP-350III",
        "Zebra ZD420",
        "Generic Thermal Printer #4",
        "That One In The Back Office",
        "Receipt Printer Formerly Known As Reliable"
    ];

    private static readonly string[] Moods =
    [
        "hostile",
        "spiritually unavailable",
        "petty",
        "technically online",
        "emotionally jammed",
        "waiting for you to believe in it",
        "possessed but passing diagnostics"
    ];

    private static readonly string[] PaperStatuses =
    [
        "loaded",
        "loaded, allegedly",
        "present but not acknowledged",
        "full roll, zero cooperation",
        "paper door closed-ish",
        "fine until the customer line forms",
        "thermal paper installed with malicious intent"
    ];

    private static readonly string[] ConnectionStatuses =
    [
        "USB, supposedly",
        "COM port chose a new identity",
        "network printer in witness protection",
        "Bluetooth pairing with Satan",
        "connected but morally opposed",
        "IP address last seen fleeing the scene",
        "Windows says connected, which is adorable"
    ];

    private static readonly string[] DriverStatuses =
    [
        "installed by a maniac in 2017",
        "working on someone else's computer",
        "driver exists, confidence does not",
        "Windows says everything is fine, which is how you know it is not",
        "vendor utility required, naturally",
        "checkbox-dependent",
        "certified haunted"
    ];

    private static readonly FailureMode[] FailureModes =
    [
        new(
            "SPIRITUALLY_UNAVAILABLE",
            "Printer perfectly fine but spiritually unavailable.",
            "Restart everything except your career.",
            "Printer passed diagnostics while actively refusing employment."
        ),
        new(
            "CHECKBOX_OF_DOOM",
            "Required checkbox is unchecked in a utility nobody remembers installing.",
            "Open the vendor settings panel and make one meaningless sacrifice.",
            "Root cause appears to be one cursed checkbox hiding behind Advanced Settings."
        ),
        new(
            "ONLINE_LEGAL_SENSE",
            "Printer is online, but only in the legal sense.",
            "Remove and re-add it until Windows feels seen.",
            "Device status says Ready. Device behavior says litigation."
        ),
        new(
            "COM_PORT_REBRAND",
            "COM port has entered its rebrand era.",
            "Try COM3, COM4, COM7, and then question your life choices.",
            "Peripheral identity appears unstable after reboot."
        ),
        new(
            "YESTERDAY_MEANT_NOTHING",
            "It printed yesterday, which apparently means nothing today.",
            "Power cycle it with the confidence of a medieval exorcist.",
            "Historical uptime has no bearing on present emotional availability."
        ),
        new(
            "THERMAL_HIEROGLYPHICS",
            "Receipt output appears to be written in thermal printer hieroglyphics.",
            "Change the code page and pretend that was obvious.",
            "Printer selected ancient cursed encoding without user consent."
        ),
        new(
            "CUSTOMER_LINE_DEBUFF",
            "Printer detected a customer waiting and immediately chose violence.",
            "Clear the queue, reboot the drawer, and avoid eye contact.",
            "Failure reproduced only under social pressure."
        ),
        new(
            "WORKS_ON_COWORKERS_MACHINE",
            "Works on your coworker's machine, because of course it does.",
            "Blame the driver. You are probably right.",
            "Issue isolated to your workstation because comedy is legal."
        )
    ];

    public static PrinterDiagnostic Generate(int rageLevel)
    {
        var failure = Pick(FailureModes);
        var chaosRating = Math.Clamp(rageLevel + Random.Next(-2, 4), 1, 10);

        return new PrinterDiagnostic(
            TicketId: $"SLPS-{Random.Next(100, 999)}",
            PrinterName: Pick(PrinterNames),
            Mood: Pick(Moods),
            PaperStatus: Pick(PaperStatuses),
            ConnectionStatus: Pick(ConnectionStatuses),
            DriverStatus: Pick(DriverStatuses),
            Diagnosis: failure.Diagnosis,
            RecommendedFix: failure.RecommendedFix,
            ChaosRating: chaosRating
        );
    }

    private static T Pick<T>(IReadOnlyList<T> values)
    {
        return values[Random.Next(values.Count)];
    }
}

public static class DiagnosticRenderer
{
    public static void Render(PrinterDiagnostic diagnostic)
    {
        Console.WriteLine();
        Console.WriteLine("🧾 Spicy Little Paper Shitter Diagnostic");
        Console.WriteLine("=======================================");
        Console.WriteLine();

        Console.WriteLine($"Ticket           : {diagnostic.TicketId}");
        Console.WriteLine($"Printer Detected : {diagnostic.PrinterName}");
        Console.WriteLine($"Mood             : {diagnostic.Mood}");
        Console.WriteLine($"Paper Status     : {diagnostic.PaperStatus}");
        Console.WriteLine($"Connection       : {diagnostic.ConnectionStatus}");
        Console.WriteLine($"Driver Status    : {diagnostic.DriverStatus}");
        Console.WriteLine();

        Console.WriteLine("Diagnosis");
        Console.WriteLine("---------");
        Console.WriteLine(diagnostic.Diagnosis);
        Console.WriteLine();

        Console.WriteLine("Recommended Fix");
        Console.WriteLine("---------------");
        Console.WriteLine(diagnostic.RecommendedFix);
        Console.WriteLine();

        Console.WriteLine($"Chaos Rating     : {diagnostic.ChaosRating}/10");
        Console.WriteLine();
    }
}

public static class TicketRenderer
{
    public static void Render(PrinterDiagnostic diagnostic)
    {
        Console.WriteLine();
        Console.WriteLine($"Ticket #{diagnostic.TicketId}");
        Console.WriteLine("================");
        Console.WriteLine();

        Console.WriteLine("Priority: Somehow Critical");
        Console.WriteLine($"Subject : Receipt printer {diagnostic.Mood}");
        Console.WriteLine();

        Console.WriteLine("Customer says:");
        Console.WriteLine("\"It was working yesterday.\"");
        Console.WriteLine();

        Console.WriteLine("Observed behavior:");
        Console.WriteLine(diagnostic.Diagnosis);
        Console.WriteLine();

        Console.WriteLine("Internal note:");
        Console.WriteLine(GetInternalNote(diagnostic));
        Console.WriteLine();

        Console.WriteLine("Recommended fix:");
        Console.WriteLine(diagnostic.RecommendedFix);
        Console.WriteLine();
    }

    private static string GetInternalNote(PrinterDiagnostic diagnostic)
    {
        if (diagnostic.ChaosRating >= 9)
            return "Printer has escalated from peripheral to hostile infrastructure.";

        if (diagnostic.ConnectionStatus.Contains("COM port", StringComparison.OrdinalIgnoreCase))
            return "COM port has emotionally relocated without submitting paperwork.";

        if (diagnostic.DriverStatus.Contains("checkbox", StringComparison.OrdinalIgnoreCase))
            return "Suspect unchecked checkbox in haunted vendor utility.";

        return "Printer detected urgency and escalated emotionally.";
    }
}

public static class ReceiptRenderer
{
    public static void Render(PrinterDiagnostic diagnostic)
    {
        Console.WriteLine();
        Console.WriteLine("================================");
        Console.WriteLine("    SPICY LITTLE PAPER CO.");
        Console.WriteLine("================================");
        Console.WriteLine($"Ticket: {diagnostic.TicketId}");
        Console.WriteLine($"Printer: {diagnostic.PrinterName}");
        Console.WriteLine("--------------------------------");
        Console.WriteLine("Item                    Price");
        Console.WriteLine("--------------------------------");
        Console.WriteLine("Driver Issue            $0.00");
        Console.WriteLine("Emotional Damage       $99.99");
        Console.WriteLine("Checkbox Nobody Saw     $4.20");
        Console.WriteLine("Thermal Violence       $13.37");
        Console.WriteLine("--------------------------------");
        Console.WriteLine("TOTAL                 $117.56");
        Console.WriteLine();
        Console.WriteLine("Payment: Tears");
        Console.WriteLine("Status : DECLINED");
        Console.WriteLine("--------------------------------");
        Console.WriteLine(WrapReceiptLine(diagnostic.Diagnosis));
        Console.WriteLine("================================");
        Console.WriteLine();
    }

    private static string WrapReceiptLine(string text)
    {
        const int width = 32;

        if (text.Length <= width)
            return text;

        var words = text.Split(' ');
        var lines = new List<string>();
        var currentLine = "";

        foreach (var word in words)
        {
            if ((currentLine + " " + word).Trim().Length > width)
            {
                lines.Add(currentLine.Trim());
                currentLine = word;
            }
            else
            {
                currentLine += " " + word;
            }
        }

        if (!string.IsNullOrWhiteSpace(currentLine))
            lines.Add(currentLine.Trim());

        return string.Join(Environment.NewLine, lines);
    }
}

public static class FixRenderer
{
    private static readonly Random Random = new();

    private static readonly string[] FixSteps =
    [
        "Restarted printer",
        "Cleared print queue",
        "Opened vendor utility",
        "Unchecked a checkbox, then checked it again",
        "Changed COM port with no confidence",
        "Reinstalled driver from suspicious ZIP file",
        "Power cycled device like a medieval exorcist",
        "Stared directly at printer until dominance was established"
    ];

    private static readonly string[] FailureResults =
    [
        "Printer now prints upside down.",
        "Printer changed ports again out of spite.",
        "Cash drawer opened, printer did not.",
        "Printer successfully printed one blank receipt as a threat.",
        "Windows reports success. Reality disagrees.",
        "Printer is now discoverable by every device except yours."
    ];

    public static void Render(PrinterDiagnostic diagnostic)
    {
        Console.WriteLine();
        Console.WriteLine("Attempting Fix");
        Console.WriteLine("==============");
        Console.WriteLine();

        var steps = FixSteps
            .OrderBy(_ => Random.Next())
            .Take(Random.Next(3, 6))
            .ToArray();

        for (var i = 0; i < steps.Length; i++)
        {
            Console.WriteLine($"Step {i + 1}: {steps[i]}");
        }

        Console.WriteLine();

        var successChance = Math.Max(15, 80 - diagnostic.ChaosRating * 7);
        var success = Random.Next(1, 101) <= successChance;

        if (success)
        {
            Console.WriteLine("Result: ✅ Printer has resumed pretending to be functional.");
            Console.WriteLine("Warning: This fix expires after the next Windows update.");
        }
        else
        {
            Console.WriteLine($"Result: ❌ {FailureResults[Random.Next(FailureResults.Length)]}");
            Console.WriteLine("Escalation: Tell the printer you are disappointed, not angry.");
        }

        Console.WriteLine();
    }
}

public static class AchievementRenderer
{
    private static readonly Random Random = new();

    private static readonly string[] Achievements =
    [
        "🏆 Works On My Machine",
        "🏆 Checkbox Goblin Survivor",
        "🏆 COM Port Speedrunner",
        "🏆 Spiritually Unavailable",
        "🏆 Thermal Violence Witness",
        "🏆 Customer Line Debuff",
        "🏆 Vendor Utility Archaeologist",
        "🏆 Restarted Everything Except Your Career"
    ];

    public static void TryRender(PrinterDiagnostic diagnostic)
    {
        if (diagnostic.ChaosRating < 8 && Random.Next(1, 101) > 35)
            return;

        Console.WriteLine("Achievement unlocked:");
        Console.WriteLine(Achievements[Random.Next(Achievements.Length)]);
        Console.WriteLine();
    }
}

public static class JsonRenderer
{
    public static void Render(PrinterDiagnostic diagnostic)
    {
        var json = JsonSerializer.Serialize(diagnostic, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        Console.WriteLine(json);
    }
}

public static class HelpRenderer
{
    public static void Render()
    {
        Console.WriteLine();
        Console.WriteLine("Spicy Little Paper Shitter");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  dotnet run");
        Console.WriteLine("  dotnet run -- --rage 10");
        Console.WriteLine("  dotnet run -- --ticket");
        Console.WriteLine("  dotnet run -- --receipt");
        Console.WriteLine("  dotnet run -- --fix");
        Console.WriteLine("  dotnet run -- --json");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --rage <1-10>   Sets printer hostility level. Default: 5");
        Console.WriteLine("  --ticket        Outputs a fake support ticket");
        Console.WriteLine("  --receipt       Outputs a fake thermal receipt");
        Console.WriteLine("  --fix           Attempts a fake repair");
        Console.WriteLine("  --json          Outputs diagnostic as JSON");
        Console.WriteLine("  --help, -h      Shows help");
        Console.WriteLine();
    }
}