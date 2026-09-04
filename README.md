# PROG6212-POE-PART-1-RaceDay-Event-Management-System
# RaceDay - Event Management System

**Module:** PROG6212 - Programming 2B  
**Student:** Lerato Maropola 
**Student Number:** ST10449273
**GitHub:** [PROG6212-POE-PART-1-RaceDay-Event-Management-System](https://github.com/Lerato-22/PROG6212-POE-PART-1-RaceDay-Event-Management-System)

## System Description

RaceDay is a full-stack web-based event management system built for the South African road running, walking, and cycling community. The platform allows Organisers to create and manage events, categories, and participant results, while Participants can browse upcoming events, enrol, track their personal performance history, and prepare for race day.

This is Part 1 of the Portfolio of Evidence which covers the full system planning phase — including the Entity Relationship Diagram, the API Endpoint Plan, and the SQL Database Script. No application code is written in this part.

## User Roles

- **Organiser** — can create, edit, and delete events, manage event categories, capture participant results, and view all event enrolments.
- **Participant** — can create an account, browse events, enter an event by selecting a category, view their own enrolments, and track their personal results.

## Part 1 Planning Documents

All planning documents are located in the `/docs` folder:

| File | Description |
|---|---|
| `ERD.drawio.png` | Entity Relationship Diagram showing all 6 entities, attributes, primary keys, foreign keys, and cardinality |
| `RaceDay-API-EndPoint-Plan..pdf` | Full API Endpoint Plan covering all 6 resource groups with HTTP method, route, role, request body, and response codes |
| `raceday_database.sql` | SQL Database Script that creates the full schema and seeds realistic sample data |


## How To Run The SQL Script

1. Open **SQL Server Management Studio (SSMS)**
2. Open the file `docs/raceday_database.sql`
3. Highlight the `CREATE DATABASE RaceDay;` line and press **F5**
4. Highlight the `USE RaceDay;` line and press **F5**
5. Select everything from the `DROP TABLE` lines to the bottom and press **F5**
6. Verify the output — you should see **Commands completed successfully** and a total count table showing all 6 tables with data

## CI/CD Screenshot

The GitHub Actions workflow validates that the `/docs` folder exists and contains all required planning files on every push to main.

![CI/CD Green Build](docs/ci_screenshot.png)

## Video Presentation

[Part 1 Video Walkthrough - YouTube](https://youtu.be/xVeU20NCDJQ?si=uLa_1UwnLc8HajmA)

## Repository Structure

```
PROG6212-POE-PART-1-RaceDay-Event-Management-System/
│
├── .github/
│   └── workflows/
│       └── validate.yml
│
├── docs/
│   ├── ERD.drawio.png
│   ├── RaceDay-API-EndPoint-Plan..pdf
│   └── raceday_database.sql
│
└── README.md
```
