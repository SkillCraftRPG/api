# SkillCraft.Api.PostgreSQL

Provides the PostgreSQL-specific infrastructure implementation for the project’s persistence, migrations, and data access concerns.

## Migrations

This project is setup to use migrations. All the commands below must be executed in the solution directory.

### Create a migration

To create a new migration, execute the following command. Do not forget to provide a migration name!

```sh
dotnet ef migrations add <YOUR_MIGRATION_NAME> --context GameContext --project src/SkillCraft.Api.PostgreSQL --startup-project src/SkillCraft.Api
```

### Remove a migration

To remove the latest unapplied migration, execute the following command.

```sh
dotnet ef migrations remove --context GameContext --project src/SkillCraft.Api.PostgreSQL --startup-project src/SkillCraft.Api
```

### Generate a script

To generate a script, execute the following command. Do not forget to provide a source migration name!

```sh
dotnet ef migrations script <SOURCE_MIGRATION> --context GameContext --project src/SkillCraft.Api.PostgreSQL --startup-project src/SkillCraft.Api
```
