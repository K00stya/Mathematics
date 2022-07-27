namespace Basics
{
    public class Test
    {
        public void Main()
        {
            UpdateField();
        }
        
        public class Field
        {
            public Cell[,] Cells;
        }
        public class Cell
        {
            
        }

        private Field _field;
        private Cell[,] _invalidСells;
        private Cell[,] _newCells;
        
        private void UpdateField()
        {
            MoveCells();
            CheckingValidityOfCells();
            RemovalInvalidCells();
            GenerateNewCells();
            FillingField();
            SaveNewFieldState();
        }

        private void MoveCells()
        {
            
        }

        private void CheckingValidityOfCells()
        {
            
        }

        private void RemovalInvalidCells()
        {
            
        }

        private void GenerateNewCells()
        {
            
        }

        private void FillingField()
        {
            
        }

        private void SaveNewFieldState()
        {
            
        }
    }

    public class Test2
    {
        public class Field
        {
            public Cell[,] Cells;
        }
        public class Cell
        {
            
        }
        
        public struct MoveInstructions
        {
            
        }
        
        public struct ValidityСriteria
        {
                
        }
        
        public struct GenerationRules
        {
            
        }
        
        public void Main()
        {
            UpdateField(new Field());
        }
        
        private void UpdateField(Field field)
        {
            var filedWithMovedCells = MoveCells(field);
            var invalidСells = GetInvalidCells(filedWithMovedCells.Cells);
            var fieldWithDeletedCells = DeleteInvalidCells(filedWithMovedCells, invalidСells);
            var newCells = GenerateNewCells(field);
            var newField = FillingField(fieldWithDeletedCells, invalidСells, newCells);
            SaveNewFieldState(newField, invalidСells, newCells);
        }

        private Field MoveCells(Field field)
        {
            return null;
        }

        private Cell[,] GetInvalidCells(Cell[,] cells)
        {
            return null;
        }

        private Field DeleteInvalidCells(Field field, Cell[,] cells)
        {
            return null;
        }

        private Cell[,] GenerateNewCells(Field field)
        {
            return null;
        }

        private Field FillingField(Field field, Cell[,] oldCells, Cell[,] newCells)
        {
            return null;
        }

        private void SaveNewFieldState(Field newField, Cell[,] invalidСells, Cell[,] newCells)
        {
            
        }
    }
    
}