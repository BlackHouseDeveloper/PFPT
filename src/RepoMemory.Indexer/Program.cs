using Microsoft.Data.Sqlite;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

// Usage: dotnet run --project src/RepoMemory.Indexer -- <inputDir> <dbPath>
if (args.Length < 2) { Console.Error.WriteLine("Usage: <inputDir> <dbPath>"); return; }
var inputDir = args[0];
var dbPath   = args[1];

if (!Directory.Exists(inputDir)) { Console.Error.WriteLine($"Missing input dir: {inputDir}"); return; }
Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(dbPath)) ?? ".");

// SQLite schema
using var db = new SqliteConnection($"Data Source={dbPath}");
db.Open();
Exec(db, """
CREATE TABLE IF NOT EXISTS documents(
  id TEXT PRIMARY KEY,
  relpath TEXT NOT NULL,
  sha TEXT NOT NULL
);
""");
Exec(db, """
CREATE TABLE IF NOT EXISTS chunks(
  doc_id TEXT NOT NULL,
  chunk_index INTEGER NOT NULL,
  text TEXT NOT NULL,
  PRIMARY KEY(doc_id, chunk_index)
);
""");

// Collect files (keep it simple: texty stuff only)
var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
{ ".md",".cs",".razor",".cshtml",".json",".yml",".yaml",".xml",".props",".targets" };

var files = Directory.EnumerateFiles(inputDir, "*.*", SearchOption.AllDirectories)
  .Where(f => allowed.Contains(Path.GetExtension(f)))
  .ToList();

Console.WriteLine($"Indexing {files.Count} files from {inputDir} into {dbPath} …");

// Insert
using var tx = db.BeginTransaction();
foreach (var file in files)
{
    var text = await File.ReadAllTextAsync(file);
    var rel  = Path.GetRelativePath(inputDir, file).Replace('\\','/');
    var docId= SHA256Hex($"{rel}:{text.Length}");
    UpsertDoc(db, docId, rel, SHA256Hex(text));

    int idx = 0;
    foreach (var chunk in Chunk(text, 900, 180)) // ~900 “tokens” (char-approx), 180 overlap
    {
        UpsertChunk(db, docId, idx++, chunk);
    }
}
tx.Commit();

Console.WriteLine("Done.");

static IEnumerable<string> Chunk(string text, int tokens, int overlap)
{
    // crude char-based chunking (≈4 chars/token)
    int size = Math.Max(1, tokens*4);
    int over = Math.Clamp(overlap*4, 0, size-1);
    for (int i=0; i<text.Length; i += (size - over))
        yield return text.Substring(i, Math.Min(size, text.Length - i));
}

static string SHA256Hex(string s)
{
    using var sha = SHA256.Create();
    var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(s));
    return Convert.ToHexString(bytes);
}

static void Exec(SqliteConnection db, string sql){ using var cmd = db.CreateCommand(); cmd.CommandText = sql; cmd.ExecuteNonQuery(); }

static void UpsertDoc(SqliteConnection db, string id, string rel, string sha)
{
    using var cmd = db.CreateCommand();
    cmd.CommandText = "INSERT OR REPLACE INTO documents(id,relpath,sha) VALUES($i,$r,$s)";
    cmd.Parameters.AddWithValue("$i", id);
    cmd.Parameters.AddWithValue("$r", rel);
    cmd.Parameters.AddWithValue("$s", sha);
    cmd.ExecuteNonQuery();
}

static void UpsertChunk(SqliteConnection db, string docId, int idx, string text)
{
    using var cmd = db.CreateCommand();
    cmd.CommandText = "INSERT OR REPLACE INTO chunks(doc_id,chunk_index,text) VALUES($d,$i,$t)";
    cmd.Parameters.AddWithValue("$d", docId);
    cmd.Parameters.AddWithValue("$i", idx);
    cmd.Parameters.AddWithValue("$t", text);
    cmd.ExecuteNonQuery();
}
