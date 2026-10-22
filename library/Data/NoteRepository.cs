
using library.Models;
using Npgsql;

namespace library.Data;

public class NoteRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public NoteRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    // 1. Get all notes
    public async Task<List<Note>> GetAllAsync()
    {
        const string sql = """
            SELECT note_id, title, body, created_on
            FROM lending.note
            ORDER BY created_on DESC, note_id DESC;
            """;

        var notes = new List<Note>();

        await using var command = _dataSource.CreateCommand(sql);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            notes.Add(new Note
            {
                NoteId = reader.GetInt64(0),
                Title = reader.GetString(1),
                Body = reader.IsDBNull(2) ? null : reader.GetString(2),
                CreatedOn = reader.GetDateTime(3)
            });
        }

        return notes;
    }

    // 2. Get one note by ID
    public async Task<Note?> GetByIdAsync(long id)
    {
        const string sql = """
            SELECT note_id, title, body, created_on
            FROM lending.note
            WHERE note_id = @id;
            """;

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("id", id);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return new Note
        {
            NoteId = reader.GetInt64(0),
            Title = reader.GetString(1),
            Body = reader.IsDBNull(2) ? null : reader.GetString(2),
            CreatedOn = reader.GetDateTime(3)
        };
    }

    // 3. Add a note
    public async Task AddAsync(Note note)
    {
        const string sql = """
            INSERT INTO lending.note (title, body)
            VALUES (@title, @body);
            """;

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("title", note.Title);
        command.Parameters.AddWithValue(
            "body", (object?)note.Body ?? DBNull.Value);

        await command.ExecuteNonQueryAsync();
    }

    // 4. Edit a note
    public async Task UpdateAsync(Note note)
    {
        const string sql = """
            UPDATE lending.note
            SET title = @title, body = @body
            WHERE note_id = @id;
            """;

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("id", note.NoteId);
        command.Parameters.AddWithValue("title", note.Title);
        command.Parameters.AddWithValue(
            "body", (object?)note.Body ?? DBNull.Value);

        await command.ExecuteNonQueryAsync();
    }

    // 5. Delete a note
    public async Task DeleteAsync(long id)
    {
        const string sql = """
            DELETE FROM lending.note
            WHERE note_id = @id;
            """;

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("id", id);

        await command.ExecuteNonQueryAsync();
    }
}
