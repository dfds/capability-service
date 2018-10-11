CREATE TABLE IF NOT EXISTS _migrations
(
    script_file     varchar(255) NOT NULL PRIMARY KEY,
    hash            varchar(64) NOT NULL,
    date_applied    timestamp NOT NULL
);
