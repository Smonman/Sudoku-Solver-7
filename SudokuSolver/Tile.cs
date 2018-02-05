using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver {
    class Tile {
        public int x;
        public int y;
        public int number;
        public bool hasNumber;

        public Tile(int x, int y) {
            this.x = x;
            this.y = y;
            number = 0;
            hasNumber = false;
        }
    }
}
