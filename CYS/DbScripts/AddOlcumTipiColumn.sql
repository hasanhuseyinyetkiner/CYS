-- Add OlcumTipi column to mobilolcum table if it doesn't exist
ALTER TABLE mobilolcum ADD COLUMN IF NOT EXISTS OlcumTipi INT DEFAULT 0;

-- Add an index on OlcumTipi for faster filtering
CREATE INDEX IF NOT EXISTS idx_mobilolcum_olcumtipi ON mobilolcum(OlcumTipi);

-- Add an index on Rfid and OlcumTipi for faster combined filtering
CREATE INDEX IF NOT EXISTS idx_mobilolcum_rfid_olcumtipi ON mobilolcum(Rfid, OlcumTipi); 