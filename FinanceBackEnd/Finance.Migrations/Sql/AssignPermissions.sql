DO $$
DECLARE
  -- configurable object names (adjust if your schema/table names differ)
  v_user_table    text := 'User';
  v_cc_table      text := 'TABLE_NAME';
  v_perm_table    text := 'PERMISSIONS_TABLE_NAME';
  v_pl_table      text := 'PermissionLevel';

  -- query values: provide one or more usernames here
  -- Example: ARRAY['alice@example.com','bob@example.com']
  v_usernames text[] := ARRAY['alice@example.com'];

  -- permission level names to assign (one or many)
  v_pl_names text[] := ARRAY['Owner'];

  -- resolved ids / sql buffer
  v_pl_ids integer[];
  v_sql text;

BEGIN
  -- Resolve permission level ids for the provided names
  EXECUTE format('SELECT array_agg("Id") FROM %I WHERE "Name" = ANY($1)', v_pl_table)
    INTO v_pl_ids
    USING v_pl_names;

  -- If none of the provided names resolved, warn and exit
  IF v_pl_ids IS NULL OR array_length(v_pl_ids,1) = 0 THEN
    RAISE NOTICE 'No permission levels found for names: %', v_pl_names;
    RETURN;
  END IF;

  -- Build and execute an INSERT referencing dynamic table names.
  -- $1 -> integer[] of permission level ids
  -- $2 -> text[] of usernames
  v_sql := format($fmt$
    INSERT INTO %I ("Id","ResourceId","UserId","PermissionLevels","CreatedAt","UpdatedAt","Deactivated")
    SELECT gen_random_uuid(), cc."Id", u."Id", $1::integer[], NOW(), NOW(), false
    FROM %I cc
    JOIN %I u ON u."Username" = ANY($2)
    WHERE NOT EXISTS (
      SELECT 1 FROM %I p WHERE p."ResourceId" = cc."Id" AND p."UserId" = u."Id"
    )
  $fmt$, v_perm_table, v_cc_table, v_user_table, v_perm_table);

  EXECUTE v_sql USING v_pl_ids, v_usernames;
END;
$$ LANGUAGE plpgsql;