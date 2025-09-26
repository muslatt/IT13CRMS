using RealEstateCRMWinForms.Data;
using RealEstateCRMWinForms.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace RealEstateCRMWinForms.ViewModels
{
    public class BoardViewModel : INotifyPropertyChanged
    {
        public const string NewBoardName = "New";
        public const string ClosedBoardName = "Closed/Done";
        private const string NewBoardColor = "#d68585";
        private const string ClosedBoardColor = "#9CA3AF";
        private const string SystemNewKey = "system:new-board";
        private const string SystemClosedKey = "system:closed-board";

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
                var boards = _context.Boards
                    .OrderBy(b => b.Order)
                    .ThenBy(b => b.CreatedAt)
                    .ToList();

                _boardsTableExists = true;

                bool changed = EnsureSystemBoards(boards);
                changed |= NormalizeBoardOrder(boards);

                if (changed)
                {
                    _context.SaveChanges();
                    boards = _context.Boards
                        .OrderBy(b => b.Order)
                        .ThenBy(b => b.CreatedAt)
                        .ToList();
                }

                Boards = new BindingList<Board>(
                    boards
                        .OrderBy(b => b.Order)
                        .ThenBy(b => b.CreatedAt)
                        .ToList());
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
                new Board { Id = 1, Name = NewBoardName, Color = NewBoardColor, Order = 1, IsDefault = true, CreatedBy = "System", IsActive = true },
                new Board { Id = 2, Name = "Offer Made", Color = "#b6c885", Order = 2, IsDefault = false, CreatedBy = "System", IsActive = true },
                new Board { Id = 3, Name = "Negotiation", Color = "#ffff99", Order = 3, IsDefault = false, CreatedBy = "System", IsActive = true },
                new Board { Id = 4, Name = "Contract Draft", Color = "#90ee90", Order = 4, IsDefault = false, CreatedBy = "System", IsActive = true },
                new Board { Id = 5, Name = ClosedBoardName, Color = ClosedBoardColor, Order = 5, IsDefault = false, CreatedBy = "System", IsActive = true }
            };

            Boards = new BindingList<Board>(fallbackBoards);
            ReorderInMemoryBoards();
        }

        public bool AddBoard(Board board)
        {
            if (!_boardsTableExists)
            {
                // Add to in-memory collection only, always before the Closed/Done board
                var maxId = Boards.Count > 0 ? Boards.Max(b => b.Id) : 0;
                board.Id = maxId + 1;
                board.IsDefault = false;
                board.IsActive = true;

                var closedIndex = Boards.ToList().FindIndex(IsClosedBoardInternal);
                if (closedIndex < 0) closedIndex = Boards.Count;

                Boards.Insert(closedIndex, board);
                ReorderInMemoryBoards();
                return true;
            }

            try
            {
                board.IsDefault = false;
                board.IsActive = true;

                // Insert before the Closed/Done board by setting order accordingly
                var closedBoard = _context.Boards.FirstOrDefault(b => b.Name == ClosedBoardName);
                if (closedBoard != null)
                {
                    board.Order = Math.Max(1, closedBoard.Order);
                    closedBoard.Order += 1;
                    closedBoard.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    var maxOrder = _context.Boards.Max(b => (int?)b.Order) ?? 0;
                    board.Order = maxOrder + 1;
                }

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

        public bool RenameBoard(Board boardToRename, string newName)
        {
            if (boardToRename == null) return false;
            if (string.IsNullOrWhiteSpace(newName)) return false;

            newName = newName.Trim();

            if (Boards.Any(b => b.Id != boardToRename.Id && b.Name.Equals(newName, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            if (!_boardsTableExists)
            {
                var board = Boards.FirstOrDefault(b => b.Id == boardToRename.Id);
                if (board == null) return false;
                board.Name = newName;
                ReorderInMemoryBoards();
                return true;
            }

            try
            {
                var board = _context.Boards.Find(boardToRename.Id);
                if (board == null) return false;

                if (board.Name.Equals(newName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                string oldName = board.Name;
                board.Name = newName;
                board.UpdatedAt = DateTime.UtcNow;

                var dealsToUpdate = _context.Deals.Where(d => d.Status == oldName).ToList();
                foreach (var deal in dealsToUpdate)
                {
                    deal.Status = newName;
                    deal.UpdatedAt = DateTime.UtcNow;
                }

                _context.SaveChanges();
                LoadBoards();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error renaming board: {ex.Message}");
                return false;
            }
        }

        public bool DeleteBoard(Board boardToDelete)
        {
            if (!_boardsTableExists)
            {
                if (IsSystemBoardInternal(boardToDelete))
                {
                    return false;
                }

                Boards.Remove(boardToDelete);
                ReorderInMemoryBoards();
                return true;
            }

            try
            {
                var board = _context.Boards.Find(boardToDelete.Id);
                if (board != null)
                {
                    if (IsSystemBoardInternal(board))
                    {
                        return false;
                    }

                    // Update any deals that reference this board to move to "New" status
                    var dealsToUpdate = _context.Deals.Where(d => d.Status == board.Name).ToList();

                    foreach (var deal in dealsToUpdate)
                    {
                        deal.Status = NewBoardName; // Always move to the system "New" board when a column is removed
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
            return Boards.FirstOrDefault(IsNewBoardInternal)
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

        public bool IsSystemBoard(Board board) => IsSystemBoardInternal(board);

        public bool IsNewBoard(Board board) => IsNewBoardInternal(board);

        public bool IsClosedBoard(Board board) => IsClosedBoardInternal(board);

        public Board? FindBoardById(int boardId) => Boards.FirstOrDefault(b => b.Id == boardId);

        public string GetNewBoardName() => Boards.FirstOrDefault(IsNewBoardInternal)?.Name ?? NewBoardName;

        public string GetClosedBoardName() => Boards.FirstOrDefault(IsClosedBoardInternal)?.Name ?? ClosedBoardName;

        private static bool IsSystemBoardInternal(Board? board) => IsNewBoardInternal(board) || IsClosedBoardInternal(board);

        private static bool IsNewBoardInternal(Board? board)
        {
            if (board == null) return false;

            if (!string.IsNullOrWhiteSpace(board.CreatedBy))
            {
                if (board.CreatedBy.Equals(SystemNewKey, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (board.CreatedBy.Equals(SystemClosedKey, StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            return board.Name.Equals(NewBoardName, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsClosedBoardInternal(Board? board)
        {
            if (board == null) return false;

            if (!string.IsNullOrWhiteSpace(board.CreatedBy))
            {
                if (board.CreatedBy.Equals(SystemClosedKey, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (board.CreatedBy.Equals(SystemNewKey, StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            return board.Name.Equals(ClosedBoardName, StringComparison.OrdinalIgnoreCase);
        }

        private bool EnsureSystemBoards(List<Board> boards)
        {
            bool changed = false;

            var newBoard = boards.FirstOrDefault(IsNewBoardInternal);
            if (newBoard == null)
            {
                newBoard = new Board
                {
                    Name = NewBoardName,
                    Color = NewBoardColor,
                    Order = 1,
                    IsDefault = true,
                    CreatedBy = SystemNewKey,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };
                _context.Boards.Add(newBoard);
                boards.Add(newBoard);
                changed = true;
            }
            else
            {
                if (EnsureSystemBoardProperties(newBoard, isNewBoard: true))
                    changed = true;
            }

            var closedBoard = boards.FirstOrDefault(IsClosedBoardInternal);
            if (closedBoard == null)
            {
                closedBoard = new Board
                {
                    Name = ClosedBoardName,
                    Color = ClosedBoardColor,
                    Order = boards.Count + 1,
                    IsDefault = false,
                    CreatedBy = SystemClosedKey,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };
                _context.Boards.Add(closedBoard);
                boards.Add(closedBoard);
                changed = true;
            }
            else
            {
                if (EnsureSystemBoardProperties(closedBoard, isNewBoard: false))
                    changed = true;
            }

            return changed;
        }

        private static bool EnsureSystemBoardProperties(Board board, bool isNewBoard)
        {
            bool changed = false;
            string expectedColor = isNewBoard ? NewBoardColor : ClosedBoardColor;
            if (!string.Equals(board.Color, expectedColor, StringComparison.OrdinalIgnoreCase))
            {
                board.Color = expectedColor;
                changed = true;
            }
            if (!board.IsActive)
            {
                board.IsActive = true;
                changed = true;
            }
            bool shouldBeDefault = isNewBoard;
            if (board.IsDefault != shouldBeDefault)
            {
                board.IsDefault = shouldBeDefault;
                changed = true;
            }
            string expectedCreatedBy = isNewBoard ? SystemNewKey : SystemClosedKey;
            if (!string.Equals(board.CreatedBy, expectedCreatedBy, StringComparison.OrdinalIgnoreCase))
            {
                board.CreatedBy = expectedCreatedBy;
                changed = true;
            }
            return changed;
        }

        private bool NormalizeBoardOrder(List<Board> boards)
        {
            bool changed = false;

            var orderedBoards = boards
                .OrderBy(b => IsNewBoardInternal(b) ? 0 : (IsClosedBoardInternal(b) ? int.MaxValue : b.Order))
                .ThenBy(b => b.CreatedAt)
                .ToList();

            int order = 1;
            foreach (var board in orderedBoards)
            {
                if (board.Order != order)
                {
                    board.Order = order;
                    changed = true;
                }

                bool shouldBeDefault = IsNewBoardInternal(board);
                if (board.IsDefault != shouldBeDefault)
                {
                    board.IsDefault = shouldBeDefault;
                    changed = true;
                }

                order++;
            }

            return changed;
        }

        private void ReorderInMemoryBoards()
        {
            if (Boards == null || Boards.Count == 0) return;

            var ordered = Boards
                .OrderBy(b => IsNewBoardInternal(b) ? 0 : (IsClosedBoardInternal(b) ? int.MaxValue : b.Order))
                .ThenBy(b => b.CreatedAt)
                .ToList();

            Boards.RaiseListChangedEvents = false;
            Boards.Clear();

            int order = 1;
            foreach (var board in ordered)
            {
                board.Order = order++;
                board.IsDefault = IsNewBoardInternal(board);
                board.IsActive = true;
                Boards.Add(board);
            }

            Boards.RaiseListChangedEvents = true;
            OnPropertyChanged(nameof(Boards));
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
                    new Board { Name = NewBoardName, Color = NewBoardColor, Order = 1, IsDefault = true, CreatedBy = "System", CreatedAt = DateTime.UtcNow, IsActive = true },
                    new Board { Name = "Offer Made", Color = "#b6c885", Order = 2, IsDefault = false, CreatedBy = "System", CreatedAt = DateTime.UtcNow, IsActive = true },
                    new Board { Name = "Negotiation", Color = "#ffff99", Order = 3, IsDefault = false, CreatedBy = "System", CreatedAt = DateTime.UtcNow, IsActive = true },
                    new Board { Name = "Contract Draft", Color = "#90ee90", Order = 4, IsDefault = false, CreatedBy = "System", CreatedAt = DateTime.UtcNow, IsActive = true },
                    new Board { Name = ClosedBoardName, Color = ClosedBoardColor, Order = 5, IsDefault = false, CreatedBy = "System", CreatedAt = DateTime.UtcNow, IsActive = true }
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
