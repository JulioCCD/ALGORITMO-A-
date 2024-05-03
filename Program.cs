// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;

class AStar
{
    // Clase para representar un punto en el espacio de búsqueda
    class Point
    {
        public int X { get; }
        public int Y { get; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    // Clase para representar un nodo en el grafo de búsqueda
    class Node
    {
        public Point Position { get; }
        public double G { get; set; } // Costo desde el inicio al nodo actual
        public double H { get; } // Heurística (estimación del costo desde el nodo actual al objetivo)
        public double F => G + H; // Costo total f

        public Node Parent { get; set; }

        public Node(Point position, double g, double h, Node parent = null)
        {
            Position = position;
            G = g;
            H = h;
            Parent = parent;
        }
    }

    // Función para calcular la distancia euclidiana entre dos puntos en el espacio
    static double EuclideanDistance(Point a, Point b)
    {
        return Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));
    }

    // Función principal para encontrar la ruta usando A*
    static List<Point> FindPath(int[,] grid, Point start, Point end)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        // Crear listas para nodos abiertos y cerrados
        var openList = new List<Node>();
        var closedList = new HashSet<Point>();

        // Agregar el nodo inicial a la lista abierta
        openList.Add(new Node(start, 0, EuclideanDistance(start, end)));

        // Iterar hasta que la lista abierta esté vacía
        while (openList.Count > 0)
        {
            // Encontrar el nodo con el menor costo total (f) en la lista abierta
            var currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].F < currentNode.F)
                {
                    currentNode = openList[i];
                }
            }

            // Mover el nodo actual de la lista abierta a la lista cerrada
            openList.Remove(currentNode);
            closedList.Add(currentNode.Position);

            // Si el nodo actual es el nodo objetivo, reconstruir y devolver la ruta
            if (currentNode.Position.X == end.X && currentNode.Position.Y == end.Y)
            {
                var path = new List<Point>();
                var current = currentNode;
                while (current != null)
                {
                    path.Add(current.Position);
                    current = current.Parent;
                }
                path.Reverse();
                return path;
            }

            // Generar sucesores del nodo actual
            foreach (var successorPoint in GetSuccessors(currentNode.Position, width, height))
            {
                // Ignorar sucesores que ya están en la lista cerrada
                if (closedList.Contains(successorPoint))
                    continue;

                // Calcular el costo g para el sucesor
                double tentativeG = currentNode.G + EuclideanDistance(currentNode.Position, successorPoint);

                // Si el sucesor no está en la lista abierta o tiene un costo g menor, agregarlo o actualizar su costo g
                var successorNode = openList.Find(node => node.Position.X == successorPoint.X && node.Position.Y == successorPoint.Y);
                if (successorNode == null || tentativeG < successorNode.G)
                {
                    if (successorNode == null)
                    {
                        successorNode = new Node(successorPoint, tentativeG, EuclideanDistance(successorPoint, end), currentNode);
                        openList.Add(successorNode);
                    }
                    else
                    {
                        successorNode.G = tentativeG;
                        successorNode.Parent = currentNode;
                    }
                }
            }
        }

        // Si no se encuentra ninguna ruta, devolver una lista vacía
        return new List<Point>();
    }

    // Función para obtener los sucesores de un punto en el espacio
    static IEnumerable<Point> GetSuccessors(Point point, int width, int height)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int newX = point.X + i;
                int newY = point.Y + j;

                // Ignorar el punto actual y los puntos fuera del límite del mapa
                if ((i == 0 && j == 0) || newX < 0 || newX >= width || newY < 0 || newY >= height)
                    continue;

                yield return new Point(newX, newY);
            }
        }
    }

    // Función para imprimir la matriz con la ruta
    static void PrintGridWithRoute(int[,] grid, List<Point> path)
    {
        for (int y = 0; y < grid.GetLength(1); y++)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                if (path.Exists(p => p.X == x && p.Y == y))
                {
                    Console.Write("o "); // Nodo en la ruta
                }
                else if (grid[x, y] == 1)
                {
                    Console.Write("# "); // Obstrucción
                }
                else
                {
                    Console.Write(". "); // Espacio vacío
                }
            }
            Console.WriteLine();
        }
    }

    static void Main(string[] args)
    {
        // Ejemplo de una matriz de 10x10
        int[,] grid = {
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 1, 0, 0, 0, 1, 0, 0},
            {0, 0, 0, 1, 0, 0, 0, 1, 0, 1},
            {1, 1, 1, 0, 0, 0, 0, 1, 0, 0},
            {0, 0, 0, 0, 1, 1, 1, 1, 0, 0},
            {0, 0, 0, 0, 1, 0, 0, 0, 0, 0},
            {1, 0, 0, 0, 1, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 1, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 1, 0, 0, 0, 0, 0}
        };

        // Punto de inicio (A) y punto final (B)
        Point startPoint = new Point(0, 7);
        Point endPoint = new Point(9, 3);

        // Encontrar la ruta desde el punto de inicio hasta el punto final
        var path = FindPath(grid, startPoint, endPoint);

        // Imprimir la matriz con la ruta
        PrintGridWithRoute(grid, path);
    }
}

