# Introduction 
Repository contains all required scripts to manage INVOICES and OCRDB SQL Server databases as Data-tier Application

##Tools:

Visual Studio 2019+ - Work with database entities
Microsoft SQL Server Management Studio 17+ - Manage data in existing Database

##Repository structure:

Sources - Folder contains Visual Studio solution with database projects

##How to for SQL Server database project:

- Project output is .dacpac database deployment artifact or SQL dump file.
- Publish project action can deploy new database from scratch or create update plan for existing database (no data loss migrations out of the box, post-deployment seed data scripts, etc.).
- Schema compare can be used to fetch or see inconsistencies between target database and database project.
- Database project IDE functionality understands and supports all main entity types(table, index, view, stored procedure, etc.).