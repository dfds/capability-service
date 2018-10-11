#!/bin/sh

TIMEOUT=1s

if [ -n "${DEBUG+set}" ]; then
    set -x
fi

set -eu

# wait for database to come online
until pg_isready -h ${PGHOST} -p ${PGPORT}; do
    echo "postgres is unavailable - waiting ${TIMEOUT}..."
    sleep $TIMEOUT
done

echo "postgres is up - preparing database migrations"

# recreate database (!! local ONLY !!)
if [ -n "${LOCAL_DEVELOPMENT+set}" ]; then
    echo -e "Creating database ${PGDATABASE}..."
    #psql -d postgres -c "DROP DATABASE IF EXISTS ${PGDATABASE};"
    psql -d postgres -c "CREATE DATABASE ${PGDATABASE};"
fi

echo -e "Creating migration table..."
psql -f ./init.sql

# run the migrations/*.sql scripts
if ls migrations/*.sql 1> /dev/null; then
    echo "Preparing migration script..."

    migration_script="/tmp/script.sql"

    cat << EOF > ${migration_script}
/* Script was generated on $(date '+%Y-%m-%d %H:%M:%S') */

EOF

    for entry in `ls migrations/*.sql | sort`
    do
        echo -e "Adding migration ${entry}"
      
        name=$(basename $entry)
        hash=$(sha1sum $entry | cut -f 1 -d ' ')

        cat << EOF >> ${migration_script}

--
-- BEG: ${name}
--
DO LANGUAGE plpgsql \$tran\$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM _migrations WHERE script_file = '${name}') THEN
EOF
        cat $entry | sed -e 's,^,\t,g' >> ${migration_script}

        cat <<EOF >> ${migration_script}

        INSERT INTO _migrations (script_file, hash, date_applied) VALUES ('${name}', '${hash}', NOW());
    END IF;
END;
\$tran\$;
--
-- END: $name
--
EOF
    done

    echo "Running migration script"
    #cat ${migration_script}
    psql -f ${migration_script}
fi
