# Spicy Little Paper Shitter

A tiny C# CLI receipt-printer chaos simulator for POS support survivors.

Built as a one-hour goblin project after job searching destroyed my higher brain function.

## Usage

dotnet run
dotnet run -- --help
dotnet run -- --list-brands
dotnet run -- --brand epson
dotnet run -- --brand star --ticket
dotnet run -- --brand zebra --receipt
dotnet run -- --brand generic --fix --rage 10
dotnet run -- --coworker sarah
dotnet run -- --zammad
dotnet run -- --json

## Modes

--ticket
Outputs a fake support ticket.

--receipt
Outputs a fake thermal receipt.

--fix
Attempts a fake repair. Results may vary. Sanity may not.

--coworker [name]
Runs the cursed works-on-their-machine test.

--zammad
Outputs a fake Zammad-style support ticket.

--json
Outputs structured diagnostic JSON.

--list-brands
Shows available printer brands.

## Brands

epson
Known crimes: COM port goblins, cash drawer beef.

star
Known crimes: network ghosts, utility software drama.

zebra
Known crimes: calibration rituals, label size curses.

generic
Known crimes: mystery drivers, USB identity crisis.

## Example

Command:

dotnet run -- --brand epson --zammad

Example output:

Zammad Ticket Export
====================

Ticket ID : SLPS-420
Customer  : Front Register
Group     : POS Support
Priority  : Somehow Critical
State     : open
Tags      : printer, pos, driver-goblin

Title
-----
Receipt printer issue - spiritually unavailable

Article
-------
Customer reports the receipt printer was working yesterday.

Printer: Epson TM-T88V
Paper: loaded, allegedly
Connection: COM port chose a new identity
Driver: checkbox-dependent

Observed behavior: Printer perfectly fine but spiritually unavailable.
Recommended action: Restart everything except your career.

Internal Note
-------------
Printer appears operational but emotionally noncompliant. Recommend ritual reboot before escalating.

## Repo Description

A tiny C# CLI receipt-printer chaos simulator for POS support survivors.

## Suggested GitHub Topics

csharp
dotnet
cli
pos
retail-tech
receipt-printer
support-tools
humor

## Why?

Because receipt printers are spicy little paper shitters that wake up every day and choose violence.