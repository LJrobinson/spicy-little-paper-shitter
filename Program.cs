using System.Text.Json;

var options = CliOptions.Parse(args);

if (options.ShowHelp)
{
    HelpRenderer.Render();
    return;
}

if (options.ListBrands)
{
    BrandRenderer.Render();
    return;
}

var diagnostic = PrinterChaosEngine.Generate(options.RageLevel, options.Brand);

if (options.Zammad)
{
    ZammadRenderer.Render(diagnostic);
    AchievementRenderer.TryRender(diagnostic);
    return;
}

if (options.Json)
{
    JsonRenderer.Render(diagnostic);
    return;
}

if (options.CoworkerName is not null)
{
    CoworkerRenderer.Render(diagnostic, options.CoworkerName);
    AchievementRenderer.TryRender(diagnostic);
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
    public bool ListBrands { get; private set; }
    public string? Brand { get; private set; }
    public string? CoworkerName { get; private set; }
    public bool Zammad { get; private set; }

    public static CliOptions Parse(string[] args)
    {
        var options = new CliOptions();
        

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--brand":
                    if (i + 1 >= args.Length)
                    {
                        Console.WriteLine("Missing --brand value. Using random printer.");
                        break;
                    }

                    options.Brand = args[i + 1].Trim().ToLowerInvariant();
                    i++;
                    break;
                
                case "--coworker":
                    options.CoworkerName = "someone else's";
                    
                    if (i + 1 < args.Length && !args[i + 1].StartsWith("--"))
                    {
                        options.CoworkerName = args[i + 1].Trim();
                        i++;
                    }

                    break;

                case "--list-brands":
                    options.ListBrands = true;
                    break;

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

                case "--zammad":
                    options.Zammad = true;
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

public sealed record PrinterProfile(
    string Brand,
    string DisplayName,
    string[] Moods,
    string[] PaperStatuses,
    string[] ConnectionStatuses,
    string[] DriverStatuses,
    FailureMode[] FailureModes
);

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

    public static PrinterDiagnostic Generate(int rageLevel, string? brand)
    {
        var profile = GetProfile(brand);
        var failure = Pick(profile.FailureModes);
        var chaosRating = Math.Clamp(rageLevel + Random.Next(-2, 4), 1, 10);

        return new PrinterDiagnostic(
            TicketId: $"SLPS-{Random.Next(100, 999)}",
            PrinterName: profile.DisplayName,
            Mood: Pick(profile.Moods),
            PaperStatus: Pick(profile.PaperStatuses),
            ConnectionStatus: Pick(profile.ConnectionStatuses),
            DriverStatus: Pick(profile.DriverStatuses),
            Diagnosis: failure.Diagnosis,
            RecommendedFix: failure.RecommendedFix,
            ChaosRating: chaosRating
        );
    }

    public static bool IsKnownBrand(string? brand)
    {
        return brand is null ||
            brand is "epson" or "star" or "zebra" or "generic";
    }

    private static PrinterProfile GetProfile(string? brand)
    {
        if (!IsKnownBrand(brand))
        {
            Console.WriteLine();
            Console.WriteLine($"Unknown brand: {brand}");
            Console.WriteLine("Using random printer instead.");
            Console.WriteLine();
        }

        return brand switch
        {
            "epson" => new PrinterProfile(
                Brand: "epson",
                DisplayName: "Epson TM-T88V",
                Moods:
                [
                    "old reliable until it smells fear",
                    "COM-port-curious",
                    "cash drawer adjacent",
                    "quietly resentful"
                ],
                PaperStatuses:
                [
                    "loaded, allegedly",
                    "roll installed by someone in a hurry",
                    "paper door closed-ish",
                    "fine until lunch rush"
                ],
                ConnectionStatuses:
                [
                    "USB, supposedly",
                    "COM port chose a new identity",
                    "serial cable doing improv",
                    "connected through pure superstition"
                ],
                DriverStatuses:
                [
                    "installed by a maniac in 2017",
                    "vendor utility required, naturally",
                    "checkbox-dependent",
                    "working after exactly three reboots"
                ],
                FailureModes:
                [
                    new(
                        "EPSON_COM_PORT_GOBLIN",
                        "Epson has reclassified itself as a different COM port for personal growth.",
                        "Try COM3, COM4, COM7, then pretend you meant to do that.",
                        "Device identity shifted after reboot. Again."
                    ),
                    new(
                        "EPSON_CASH_DRAWER_BEEF",
                        "Printer works, but the cash drawer is now involved emotionally.",
                        "Check the drawer kick settings and question why this is printer-adjacent.",
                        "Cash drawer dependency chain has entered comedy territory."
                    )
                ]
            ),

            "star" => new PrinterProfile(
                Brand: "star",
                DisplayName: "Star Micronics TSP100",
                Moods:
                [
                    "utility-software haunted",
                    "passive aggressive",
                    "driver dramatic",
                    "firmware theatrical"
                ],
                PaperStatuses:
                [
                    "loaded but offended",
                    "full roll, zero cooperation",
                    "paper present in theory",
                    "thermal roll seated with vibes only"
                ],
                ConnectionStatuses:
                [
                    "network printer in witness protection",
                    "USB handshake went to lunch",
                    "connected to the wrong version of reality",
                    "IP address last seen fleeing the scene"
                ],
                DriverStatuses:
                [
                    "TSP100 utility has opinions",
                    "driver exists, confidence does not",
                    "Windows says fine, which is adorable",
                    "installed twice, somehow less functional"
                ],
                FailureModes:
                [
                    new(
                        "STAR_UTILITY_POSSESSION",
                        "Star utility opened successfully and somehow made the situation worse.",
                        "Close the utility, reopen it as administrator, and offer tribute.",
                        "Vendor utility changed one setting nobody requested."
                    ),
                    new(
                        "STAR_NETWORK_GHOST",
                        "Printer is visible on the network but refuses to be known.",
                        "Reassign the IP address and whisper the subnet mask correctly.",
                        "Network discovery succeeded. Printing did not."
                    )
                ]
            ),

            "zebra" => new PrinterProfile(
                Brand: "zebra",
                DisplayName: "Zebra ZD420",
                Moods:
                [
                    "label-printer elite",
                    "calibration obsessed",
                    "professionally difficult",
                    "tiny industrial diva"
                ],
                PaperStatuses:
                [
                    "labels loaded backwards, probably",
                    "gap sensor emotionally confused",
                    "calibration required because joy is illegal",
                    "media present but spiritually misaligned"
                ],
                ConnectionStatuses:
                [
                    "USB pretending to be enterprise infrastructure",
                    "networked like a tiny mainframe",
                    "connected but demanding calibration",
                    "driver stack wearing a hard hat"
                ],
                DriverStatuses:
                [
                    "ZDesigner driver selected violence",
                    "calibration wizard required",
                    "darkness setting became a lifestyle",
                    "driver installed, label size wrong forever"
                ],
                FailureModes:
                [
                    new(
                        "ZEBRA_CALIBRATION_RITUAL",
                        "Zebra refuses to print until the label gap sensor is emotionally validated.",
                        "Run calibration and pretend the blinking light pattern is documentation.",
                        "Media calibration required before basic cooperation resumes."
                    ),
                    new(
                        "ZEBRA_LABEL_SIZE_CURSE",
                        "Label size is wrong in exactly three different places.",
                        "Fix it in Windows, the app, and the driver because one setting would be too kind.",
                        "Conflicting label dimensions detected across reality layers."
                    )
                ]
            ),

            "generic" => new PrinterProfile(
                Brand: "generic",
                DisplayName: "Generic Thermal Printer #4",
                Moods:
                [
                    "driverless and proud",
                    "suspiciously cheap",
                    "haunted by AliExpress",
                    "legally a printer"
                ],
                PaperStatuses:
                [
                    "loaded with bargain-bin confidence",
                    "paper detected by vibes",
                    "roll present, dignity absent",
                    "thermal paper of unknown origin"
                ],
                ConnectionStatuses:
                [
                    "USB device recognized as emotional damage",
                    "unknown device, obviously",
                    "COM port generated by roulette wheel",
                    "connected through a cable from a drawer"
                ],
                DriverStatuses:
                [
                    "downloaded from a website with six popups",
                    "signed by absolutely nobody",
                    "works only after disabling hope",
                    "generic driver doing community service"
                ],
                FailureModes:
                [
                    new(
                        "GENERIC_DRIVER_DUMPSTER",
                        "Generic driver installed successfully, which is the first red flag.",
                        "Try a different generic driver and prepare for lateral disappointment.",
                        "Driver compatibility achieved only in a technical sense."
                    ),
                    new(
                        "GENERIC_UNKNOWN_DEVICE",
                        "Windows recognizes this printer as 'USB Printing Support' and nothing else.",
                        "Install the mystery driver and hope it was not bundled with a toolbar.",
                        "Device identity unavailable. Morale also unavailable."
                    )
                ]
            ),

            _ => new PrinterProfile(
                Brand: "random",
                DisplayName: Pick(PrinterNames),
                Moods: Moods,
                PaperStatuses: PaperStatuses,
                ConnectionStatuses: ConnectionStatuses,
                DriverStatuses: DriverStatuses,
                FailureModes: FailureModes
            )
        };
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

public static class BrandRenderer
{
    public static void Render()
    {
        Console.WriteLine();
        Console.WriteLine("Available Printer Brands");
        Console.WriteLine("========================");
        Console.WriteLine();

        Console.WriteLine("epson");
        Console.WriteLine("  Old reliable until it smells fear.");
        Console.WriteLine("  Known crimes: COM port goblins, cash drawer beef.");
        Console.WriteLine();

        Console.WriteLine("star");
        Console.WriteLine("  Utility software haunted by retail trauma.");
        Console.WriteLine("  Known crimes: network ghosts, driver drama.");
        Console.WriteLine();

        Console.WriteLine("zebra");
        Console.WriteLine("  Tiny industrial diva with calibration demands.");
        Console.WriteLine("  Known crimes: label size curses, sensor rituals.");
        Console.WriteLine();

        Console.WriteLine("generic");
        Console.WriteLine("  Legally a printer. Emotionally a lawsuit.");
        Console.WriteLine("  Known crimes: mystery drivers, USB identity crisis.");
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

public static class CoworkerRenderer
{
    private static readonly Random Random = new();

    private static readonly string[] Conclusions =
    [
        "This is now somehow your fault.",
        "The printer has chosen favorites.",
        "The issue cannot be reproduced because comedy is cruel.",
        "Your workstation has failed the vibe check.",
        "The printer respects them more than you.",
        "Escalation denied. Printer has plausible deniability."
    ];

    private static readonly string[] SuggestedNextSteps =
    [
        "Swap computers and deny everything.",
        "Ask what checkbox they have that you apparently do not.",
        "Reinstall the driver while maintaining aggressive eye contact.",
        "Move their printer profile to your machine and hope it brings the blessing.",
        "Blame Windows updates. This is legally safe.",
        "Open Device Manager and pretend this is a science."
    ];

    public static void Render(PrinterDiagnostic diagnostic, string coworkerName)
    {
        var displayName = NormalizeName(coworkerName);

        Console.WriteLine();
        Console.WriteLine("Coworker Test");
        Console.WriteLine("=============");
        Console.WriteLine();

        Console.WriteLine($"Printer : {diagnostic.PrinterName}");
        Console.WriteLine($"Tested on: {displayName} machine");
        Console.WriteLine("Result   : ✅ Works perfectly");
        Console.WriteLine();

        Console.WriteLine("Original issue:");
        Console.WriteLine(diagnostic.Diagnosis);
        Console.WriteLine();

        Console.WriteLine("Conclusion:");
        Console.WriteLine(Conclusions[Random.Next(Conclusions.Length)]);
        Console.WriteLine();

        Console.WriteLine("Suggested next step:");
        Console.WriteLine(SuggestedNextSteps[Random.Next(SuggestedNextSteps.Length)]);
        Console.WriteLine();
    }

    private static string NormalizeName(string coworkerName)
    {
        if (coworkerName.Equals("someone else's", StringComparison.OrdinalIgnoreCase))
            return "someone else's";

        if (coworkerName.EndsWith("'s", StringComparison.OrdinalIgnoreCase))
            return coworkerName;

        return $"{coworkerName}'s";
    }
}

public static class ZammadRenderer
{
    private static readonly Random Random = new();

    private static readonly string[] Customers =
    [
        "Front Register",
        "Back Office",
        "Inventory Station",
        "Manager Terminal",
        "Reception Desk",
        "The One Computer Everyone Hates"
    ];

    private static readonly string[] Priorities =
    [
        "Normal",
        "High",
        "Somehow Critical",
        "Manager Is Watching",
        "Customer Line Forming",
        "Spiritually Urgent"
    ];

    private static readonly string[] Tags =
    [
        "printer",
        "pos",
        "thermal-chaos",
        "driver-goblin",
        "retail-tech",
        "works-yesterday"
    ];

    public static void Render(PrinterDiagnostic diagnostic)
    {
        Console.WriteLine();
        Console.WriteLine("Zammad Ticket Export");
        Console.WriteLine("====================");
        Console.WriteLine();

        Console.WriteLine($"Ticket ID : {diagnostic.TicketId}");
        Console.WriteLine($"Customer  : {Pick(Customers)}");
        Console.WriteLine($"Group     : POS Support");
        Console.WriteLine($"Priority  : {Pick(Priorities)}");
        Console.WriteLine($"State     : open");
        Console.WriteLine($"Tags      : {string.Join(", ", Tags.OrderBy(_ => Random.Next()).Take(3))}");
        Console.WriteLine();

        Console.WriteLine("Title");
        Console.WriteLine("-----");
        Console.WriteLine($"Receipt printer issue - {diagnostic.Mood}");
        Console.WriteLine();

        Console.WriteLine("Article");
        Console.WriteLine("-------");
        Console.WriteLine("Customer reports the receipt printer was working yesterday.");
        Console.WriteLine();
        Console.WriteLine($"Printer: {diagnostic.PrinterName}");
        Console.WriteLine($"Paper: {diagnostic.PaperStatus}");
        Console.WriteLine($"Connection: {diagnostic.ConnectionStatus}");
        Console.WriteLine($"Driver: {diagnostic.DriverStatus}");
        Console.WriteLine();
        Console.WriteLine($"Observed behavior: {diagnostic.Diagnosis}");
        Console.WriteLine($"Recommended action: {diagnostic.RecommendedFix}");
        Console.WriteLine();

        Console.WriteLine("Internal Note");
        Console.WriteLine("-------------");
        Console.WriteLine("Printer appears operational but emotionally noncompliant. Recommend ritual reboot before escalating.");
        Console.WriteLine();
    }

    private static string Pick(IReadOnlyList<string> values)
    {
        return values[Random.Next(values.Count)];
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
        Console.WriteLine("  dotnet run -- --brand epson --ticket");
        Console.WriteLine("  dotnet run -- --list-brands");
        Console.WriteLine("  dotnet run -- --coworker sarah");
        Console.WriteLine("  dotnet run -- --zammad");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --rage <1-10>  Sets printer hostility level. Default: 5");
        Console.WriteLine("  --ticket       Outputs a fake support ticket");
        Console.WriteLine("  --receipt      Outputs a fake thermal receipt");
        Console.WriteLine("  --fix          Attempts a fake repair");
        Console.WriteLine("  --json         Outputs diagnostic as JSON");
        Console.WriteLine("  --brand <name> Chooses printer brand: epson, star, zebra, generic");
        Console.WriteLine("  --list-brands  Shows available printer brands");
        Console.WriteLine("  --coworker [name] Runs the cursed works-on-their-machine test");
        Console.WriteLine("  --zammad       Outputs a fake Zammad-style support ticket");
        Console.WriteLine("  --help, -h     Shows help");
        Console.WriteLine();
    }
}