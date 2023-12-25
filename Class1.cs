
using System;
using System.Collections.Generic;
using System.Text;

namespace Sudoku
{
    // Deze klasse representeert het Sudoku Puzzel Bord
    class SudokuPuzzle
    {
        private int[,] board;

        // De Constructor dat de Sudoku Puzzel initialiseert met een input string
        public SudokuPuzzle(string input)
        {
            board = new int[9, 9]; // 2D-array om inputstring in te verwerken
            string[] values = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // Vul het Sudoku Boord in vanuit de input string
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {

                    board[i, j] = int.Parse(values[i * 9 + j]);
                }
            }
        }

        // Roep het Sudoku Boord op
        public int[,] GetBoard()
        {
            return board;
        }

    }

    // Bevat de operators, algoritmes en methodes voor het oplossen van de Sudoku Puzzel
    public class Operators
    {
        private SudokuPuzzle? sudokuBoard; //nullable field
        public List<(int, int)> fixedNumberIndices;

        // Operator Constructor Methode
        public Operators()
        {
            fixedNumberIndices = new List<(int, int)>(); //initiliseer de list
        }

        // Constructor met Sudoku Puzzel parameter
        private Operators(SudokuPuzzle board)
        {
            sudokuBoard = board;
            fixedNumberIndices = new List<(int, int)>();
        }

        // Genereer een Random Getal tussen 1 en 9
        public int GetRandomNumber()
        {
            Random random = new Random();
            return random.Next(1, 10);
        }

        // Initialiseer het Sudoku Boord met gefixeerde getallen en vul de ontbrekende getallen in
        public void InitiateBoard(int[,] board)
        {
            FillInitialNumbers(board); //behou gefixeerde getallen
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (board[i, j] != 0)
                    {
                        fixedNumberIndices.Add((i, j));
                    }
                }
            }
            FillRemainingCells(board); //vul overige cellen met willekeurige getallen [1-9]
            PrintBoard(board);
        }

        // Vul de initiele gefixeerde getallen in op het Sudoku Bord
        public void FillInitialNumbers(int[,] board)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (board[i, j] != 0)
                    {
                        //gefixeerd getal; vul in op het bord
                    }
                }
            }
        }


        // Vul de overige lege plaatsen op het Sudoku Boord in met Random Getallen
        public void FillRemainingCells(int[,] board)
        {
            for (int blockRow = 0; blockRow < 9; blockRow += 3) //selecteer blok
            {
                for (int blockCol = 0; blockCol < 9; blockCol += 3) //selecteer blok
                {
                    for (int num = 1; num <= 9; num++) //vul de getallen [1-9] in op de lege posities
                    {
                        for (int row = 0; row < 3; row++) //loop door blok
                        {
                            for (int col = 0; col < 3; col++) //loop door blok
                            {
                                if (board[blockRow + row, blockCol + col] == 0) //als er op deze positie nu een nul staat 
                                {
                                    do //loop genereert random getallen totdat het een getal genereert wat nog niet voorkomt 
                                    {
                                        num = GetRandomNumber();
                                    } while (NotinBlock(board, blockRow, blockCol, num)); //bool gebruikt om te checken of getallen nog niet voorkomt in blok

                                    board[blockRow + row, blockCol + col] = num;
                                }
                            }
                        }
                    }
                }
            }
        }

        // Check of een getal nog niet voorkomt in het huidige blok
        public bool NotinBlock(int[,] board, int blockRow, int blockCol, int num)
        {
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    if (board[blockRow + row, blockCol + col] == num)
                    {
                        return true; // Return waarde TRUE voor als het getal zich al wel in het 3x3 blok bevindt
                    }
                }
            }
            return false; //getal bevindt zich niet in huidige blok
        }

        // Print het Sudoku Boord
        public void PrintBoard(int[,] board)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Console.Write(board[i, j] + " ");

                    if ((j + 1) % 3 == 0 && j < 8) //vertikale lijnen
                    {
                        Console.Write("| ");
                    }
                }

                Console.WriteLine();

                if ((i + 1) % 3 == 0 && i < 8) //horizontale lijnen 
                {
                    Console.WriteLine("------+-------+------");
                }
            }

            Console.WriteLine();
        }

        // Evalueer de score van het boord (het aantal lege cellen)
        public int EvaluationFunction(int[,] board)
        {
            int score = 0;
            HashSet<(int, int)> countedZeros = new HashSet<(int, int)>(); //gebruik hash set om doubles te voorkomen 

            for (int row = 0; row < 9; row++) //loop door bord en tel nullen 1 maal
            {
                for (int col = 0; col < 9; col++)
                {
                    if (board[row, col] == 0 && countedZeros.Add((row, col)))
                    {
                        score++;
                    }
                }
            }
            return score;
        }


        public int[,] SearchOperator(int[,] board)
        {
        int[,] newBoard = (int[,])board.Clone();

            return newBoard;
        }

        // Check of een getal gefixeerd is of niet
        private bool IsFixedIndex(List<(int, int)> fixedIndices, int row, int col)
        {
            return fixedIndices.Contains((row, col));
        }

        // Genereer een Successor Bord met behulp van de zoekoperator (Random Swap operatie)
        public int[,] GetSuccessor(int[,] board)
        {
            int[,] successor = (int[,])board.Clone(); //kloon huidige toestand 

            return SearchOperator(successor); //toepassen zoekoperator om successor te genereren 
        }

    }

    // Het Hill climbing algoritme voor het oplossen van de Sudoku Puzzel
    public class ChrononologicalBT
    {
        public Operators operators;
        public Random random;

        // Constructor
        public ChrononologicalBT()
        {
            operators = new Operators();
            random = new Random();
        }

        //Huidige iteratie
        public int Iteration { get; private set; } = 0;


    }

    public class ForwardChecking
    {
        public Operators operators;
        public Random random;

        // Constructor
        public ForwardChecking()
        {
            operators = new Operators();
            random = new Random();
        }

        //Huidige iteratie
        public int Iteration { get; private set; } = 0;


    }

    class Program
    {
        static void Main()
        {
            string input = Console.ReadLine(); ////input format: 0 0 3 0 2 0 6 0 0 9 0 0 3 0 5 0 0 1 0 0 1 8 0 6 4 0 0 0 0 8 1 0 2 9 0 0 7 0 0 0 0 0 0 0 8 0 0 6 7 0 8 2 0 0 0 0 2 6 0 9 5 0 0 8 0 0 2 0 3 0 0 9 0 0 5 0 1 0 3 0 0';
            SudokuPuzzle puzzle = new SudokuPuzzle(input);
            int[,] initialBoard = puzzle.GetBoard();
            int[,] currentBoard = (int[,])initialBoard.Clone();
            ChrononologicalBT AlgorithmBT = new ChrononologicalBT();
            ForwardChecking AlgorithmFWC = new ForwardChecking();
            Operators operators = new Operators(); //maak een instance aan van Operators 

            Console.WriteLine("Initial Sudoku Puzzle (fixed values):");
            operators.PrintBoard(initialBoard); //instance gebruiken om de methode aan te roepen

            Console.WriteLine("Initial Sudoku Puzzle (filled in):");
            operators.InitiateBoard(currentBoard); //initialiseer bord; random getallen invullen 

            int maxIterations = 100000;
            int randomWalkSteps = 2;

            //int[,] finalBoard = ;

            //int finalscore =  //evalueer eindresultaat;

            int initialScore = operators.EvaluationFunction(initialBoard); //evalueer initiele bord
            Console.WriteLine($"Initial Score: {initialScore}");



        }

    }

}



