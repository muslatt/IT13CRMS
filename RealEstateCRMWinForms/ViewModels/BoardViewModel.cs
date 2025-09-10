using RealEstateCRMWinForms.Data;
using RealEstateCRMWinForms.Models;
using System.ComponentModel;

namespace RealEstateCRMWinForms.ViewModels
{
    public class BoardViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _context;
        public BindingList<Board> Boards { get; set; }
        private bool _boardsTableExists = false;

        public event PropertyChangedEventHandler? PropertyChanged;

        public BoardViewModel()
        {
            _context = DbContextHelper.CreateDbContext();
            Boards = new BindingList<Board>();
            LoadBoards();
        }

        public void LoadBoards()
        {
            try
            {
                // Check if Boards table exists
                var boards = _context.Boards.OrderBy(b => b.Order).ThenBy(b => b.CreatedAt).ToList();
                Boards = new BindingList<Board>(boards);
                _boardsTableExists = true;
                OnPropertyChanged(nameof(Boards));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading boards (likely table doesn't exist): {ex.Message}");
                _boardsTableExists = false;
                
                // Create fallback in-memory boards
                CreateFallbackBoards();
                OnPropertyChanged(nameof(Boards));
            }
        }

        private void CreateFallbackBoards()
        {
            var fallbackBoards = new List<Board>
            {
                new Board { Id = 1, Name = "New", Color = "#d68585", Order = 1, IsDefault = true, CreatedBy = "System" },
                new Board { Id = 2, Name = "Offer Made", Color = "#b6c885", Order = 2, IsDefault = false, CreatedBy = "System" },
                new Board { Id = 3, Name = "Negotiation", Color = "#ffff99", Order = 3, IsDefault = false, CreatedBy = "System" },
                new Board { Id = 4, Name = "Contract Draft", Color = "#90ee90", Order = 4, IsDefault = false, CreatedBy = "System" }
            };

            Boards = new BindingList<Board>(fallbackBoards);
        }

        public bool AddBoard(Board board)
        {
            if (!_boardsTableExists)
            {
                // Add to in-memory collection only
                var maxId = Boards.Count > 0 ? Boards.Max(b => b.Id) : 0;
                board.Id = maxId + 1;
                board.Order = Boards.Count + 1;
                
                // Clear existing defaults
                foreach (var existingDefault in Boards.Where(b => b.IsDefault))
                {
                    existingDefault.IsDefault = false;
                }
                
                board.IsDefault = true;
                Boards.Add(board);
                return true;
            }

            try
            {
                // If this is the first board or explicitly set as default, make it default
                if (!_context.Boards.Any() || board.IsDefault)
                {
                    // Clear existing defaults
                    var existingDefaults = _context.Boards.Where(b => b.IsDefault).ToList();
                    foreach (var existingDefault in existingDefaults)
                    {
                        existingDefault.IsDefault = false;
                        existingDefault.UpdatedAt = DateTime.UtcNow;
                    }
                    
                    board.IsDefault = true;
                }

                // Set the order for new board
                var maxOrder = _context.Boards.Max(b => (int?)b.Order) ?? 0;
                board.Order = maxOrder + 1;

                _context.Boards.Add(board);
                _context.SaveChanges();
                LoadBoards();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding board: {ex.Message}");
                return false;
            }
        }

        public bool UpdateBoard(Board boardToUpdate)
        {
            if (!_boardsTableExists)
            {
                var existingBoard = Boards.FirstOrDefault(b => b.Id == boardToUpdate.Id);
                if (existingBoard != null)
                {
                    var index = Boards.IndexOf(existingBoard);
                    Boards[index] = boardToUpdate;
                    return true;
                }
                return false;
            }

            try
            {
                var board = _context.Boards.Find(boardToUpdate.Id);
                if (board != null)
                {
                    _context.Entry(board).CurrentValues.SetValues(boardToUpdate);
                    board.UpdatedAt = DateTime.UtcNow;
                    _context.SaveChanges();
                    LoadBoards();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating board: {ex.Message}");
                return false;
            }
        }

        public bool DeleteBoard(Board boardToDelete)
        {
            if (!_boardsTableExists)
            {
                // Only protect the "New" board from deletion
                if (boardToDelete.Name == "New")
                {
                    return false;
                }

                Boards.Remove(boardToDelete);
                return true;
            }

            try
            {
                var board = _context.Boards.Find(boardToDelete.Id);
                if (board != null)
                {
                    // Only protect the "New" board from deletion
                    if (board.Name == "New")
                    {
                        return false;
                    }

                    // Update any deals that reference this board to move to "New" status
                    var dealsToUpdate = _context.Deals.Where(d => d.Status == board.Name).ToList();
                    
                    foreach (var deal in dealsToUpdate)
                    {
                        deal.Status = "New"; // Always move to "New" when a board is deleted
                        deal.UpdatedAt = DateTime.UtcNow;
                    }

                    _context.Boards.Remove(board);
                    _context.SaveChanges();
                    LoadBoards();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting board: {ex.Message}");
                return false;
            }
        }

        public Board? GetDefaultBoard()
        {
            return Boards.FirstOrDefault(b => b.IsDefault) 
                ?? Boards.OrderBy(b => b.Order).FirstOrDefault();
        }

        public void SetDefaultBoard(int boardId)
        {
            if (!_boardsTableExists)
            {
                // Clear existing defaults
                foreach (var board in Boards.Where(b => b.IsDefault))
                {
                    board.IsDefault = false;
                }

                // Set new default
                var newDefault = Boards.FirstOrDefault(b => b.Id == boardId);
                if (newDefault != null)
                {
                    newDefault.IsDefault = true;
                }
                return;
            }

            try
            {
                // Clear existing defaults
                var existingDefaults = _context.Boards.Where(b => b.IsDefault).ToList();
                foreach (var board in existingDefaults)
                {
                    board.IsDefault = false;
                    board.UpdatedAt = DateTime.UtcNow;
                }

                // Set new default
                var newDefault = _context.Boards.Find(boardId);
                if (newDefault != null)
                {
                    newDefault.IsDefault = true;
                    newDefault.UpdatedAt = DateTime.UtcNow;
                }

                _context.SaveChanges();
                LoadBoards();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting default board: {ex.Message}");
            }
        }

        public void InitializeDefaultBoards()
        {
            if (!_boardsTableExists)
            {
                // Boards are already created as fallback
                return;
            }

            try
            {
                // Check if boards already exist
                if (_context.Boards.Any()) return;

                var defaultBoards = new List<Board>
                {
                    new Board { Name = "New", Color = "#d68585", Order = 1, IsDefault = true, CreatedBy = "System" },
                    new Board { Name = "Offer Made", Color = "#b6c885", Order = 2, IsDefault = false, CreatedBy = "System" },
                    new Board { Name = "Negotiation", Color = "#ffff99", Order = 3, IsDefault = false, CreatedBy = "System" },
                    new Board { Name = "Contract Draft", Color = "#90ee90", Order = 4, IsDefault = false, CreatedBy = "System" }
                };

                _context.Boards.AddRange(defaultBoards);
                _context.SaveChanges();
                LoadBoards();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing default boards: {ex.Message}");
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}