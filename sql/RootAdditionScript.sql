Insert into "Users" ("Id", "Email", "Password", "AccountType", "VerifiedRoles", "ClaimedRoles") 
values ('00000000-0000-0000-0000-000000000000', :'ROOT_EMAIL', :'ROOT_PASS', 0, ARRAY[0]::integer[], Array[]::integer[])
ON CONFLICT DO UPDATE;