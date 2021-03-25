using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using System.Xml.Linq;
using CamelUpCalculator.Calculators;

namespace CamelUpCalculator
{
    public class MatrixRepository
    {
        private readonly string _filepathRoot;
        private MatrixCalculator _matrixCalculator;
        private readonly List<int> _splitMatrices;
        private readonly int _partialCount;
        private readonly int _maxnoOfMatrices;
        private Func<Dictionary<int, Dictionary<int, Dictionary<int, double>>>> _generateInitialMatrix;

        public MatrixRepository(MatrixCalculator matrixCalculator, Func<Dictionary<int, Dictionary<int, Dictionary<int, double>>>> GenerateInitialMatrix)
        {
            _filepathRoot = Directory.GetCurrentDirectory() + "\\Matrix";
            _matrixCalculator = matrixCalculator;
            _splitMatrices = Constants.NumberOfCamels > 4 ? new List<int> { 2, 3 } : new List<int>{2, 3};
            _partialCount = Constants.NumberOfCamels > 4 ? 204 : 12;
            _maxnoOfMatrices = Constants.NumberOfCamels > 4 ? 6 : 6;
            _generateInitialMatrix = GenerateInitialMatrix;

            InitialiseEndMatrix();
        }

        public Dictionary<int, Dictionary<int, Dictionary<int, double>>> EndMatrix { get; set; }

        private void InitialiseMatrix()
        {
            if (!File.Exists(GetFilepath(1)))
            {
                var matrix = _generateInitialMatrix();
                WriteMatrix(GetFilepath(1), matrix);
            }
        }

        private void GenerateMatrices()
        {
            for (int i = 1; i < _maxnoOfMatrices; i++)
            {
                // Both files exist fully
                if (File.Exists(GetFilepath(i)) && File.Exists(GetFilepath(i + 1)))
                {
                    continue;
                }

                // i exists but not i + 1, i, i + 1 not partial
                if (File.Exists(GetFilepath(i)) && !File.Exists(GetFilepath(i + 1)) && !IsPartial(i + 1))
                {
                    var ithMatrix = LoadMatrix(GetFilepath(i));
                    var squaredMatrix = _matrixCalculator.SquareMatrix(ithMatrix);
                    WriteMatrix(GetFilepath(i + 1), squaredMatrix);
                    continue;
                }

                // i exists, but not i + 1, i + 1 partial
                if (File.Exists(GetFilepath(i)) && IsPartial(i + 1) && !File.Exists(GetPartialFilePath(i + 1, _partialCount - 1)))
                {
                    for (int j = 0; j < _partialCount; j++)
                    {
                        if (!File.Exists(GetPartialFilePath(i + 1, j)))
                        {
                            var ithMatrix = LoadMatrix(GetFilepath(i));
                            var ijthPartialMatrix = _matrixCalculator.PartialSquareMatrix(ithMatrix, j, _partialCount);
                            WriteMatrix(GetPartialFilePath(i + 1, j), ijthPartialMatrix);
                        }
                    }
                    continue;
                }

                // i exists, but not i + 1, i, i + 1 partial
                if (File.Exists(GetPartialFilePath(i, _partialCount - 1)) && IsPartial(i + 1) && !File.Exists(GetPartialFilePath(i + 1, _partialCount - 1)))
                {

                    var unitMatrix = LoadMatrix(GetFilepath(1));

                    for (int j = 0; j < _partialCount; j++)
                    {
                        if (!File.Exists(GetPartialFilePath(i + 1, j)))
                        {
                            var ijthPartialMatrix = LoadMatrix(GetPartialFilePath(i, j));
                            var nextPartialMatrix = _matrixCalculator.PartialUnitMatrix(unitMatrix, ijthPartialMatrix, j, _partialCount);
                            WriteMatrix(GetPartialFilePath(i + 1, j), nextPartialMatrix);
                        }
                    }
                    continue;
                }

                // i exists but not i + 1, i partial
                if (File.Exists(GetPartialFilePath(i, _partialCount - 1)) && !IsPartial(i + 1) && !File.Exists(GetFilepath(i + 1)))
                {

                    var ithMatrix = LoadPartialMatrices(GetFilepath(i));
                    var squaredMatrix = _matrixCalculator.SquareMatrix(ithMatrix);
                    WriteMatrix(GetFilepath(i + 1), squaredMatrix);
                    continue;
                }
            }
        }

        private string GetFilepath(int index)
        {
            return _filepathRoot + Constants.NumberOfCamels + "," + index;
        }

        private string GetPartialFilePath(int index, int partialIndex)
        {
            return _filepathRoot + Constants.NumberOfCamels + "," + index + "," + partialIndex;
        }

        private bool IsPartial(int matrix)
        {
            return _splitMatrices.Contains(matrix);
        }

        private void InitialiseEndMatrix()
        {
            var filepath = GetFilepath(_maxnoOfMatrices);
            if (!File.Exists(filepath))
            {
                InitialiseMatrix();
                GenerateMatrices();
            }
            EndMatrix = LoadMatrix(filepath);
        }

        private void WriteMatrix(string filepath, Dictionary<int, Dictionary<int, Dictionary<int, double>>> matrix)
        {
            XElement matrixElement = GetMatrixElement(matrix);

            XmlSerializer writer = new XmlSerializer(typeof(XElement));

            FileStream file = File.Create(filepath);
            writer.Serialize(file, matrixElement);
            file.Close();
            matrixElement = null;
        }

        private XElement GetMatrixElement(Dictionary<int, Dictionary<int, Dictionary<int, double>>> matrix)
        {
            XElement matrixElement = new XElement("matrix");
            var rowKeys = matrix.Keys;
            foreach (var rowKey in rowKeys)
            {
                var rowElement = GetRowElement(rowKey, matrix[rowKey]);
                matrixElement.Add(rowElement);
            }

            return matrixElement;
        }

        private XElement GetRowElement(int rowKey, Dictionary<int, Dictionary<int, double>> column)
        {
            XElement rowElement = new XElement(rowKey.ToXMLString("r"));
            var columnKeys = column.Keys;
            foreach (var columnKey in columnKeys)
            {
                var columnElement = GetColumnElement(columnKey, column[columnKey]);
                rowElement.Add(columnElement);
            }

            return rowElement;
        }

        private XElement GetColumnElement(int columnKey, Dictionary<int, double> permutations)
        {
            XElement columnElement = new XElement(columnKey.ToXMLString("c"));
            var permutationKeys = permutations.Keys;
            foreach (var permutationKey in permutationKeys)
            {
                var permutationElement = new XElement(permutationKey.ToXMLString("p"));
                permutationElement.Add(new XAttribute("count", permutations[permutationKey]));
                columnElement.Add(permutationElement);
            }

            return columnElement;
        }

        private Dictionary<int, Dictionary<int, Dictionary<int, double>>> LoadMatrix(string filepath)
        {
            var matrixElement = XElement.Load(filepath);

            var matrix = new Dictionary<int, Dictionary<int, Dictionary<int, double>>>();
            foreach (var rowElement in matrixElement.Elements())
            {
                var columnDictionary = new Dictionary<int, Dictionary<int, double>>();
                foreach (var columnElement in rowElement.Elements())
                {
                    var permutationDictionary = new Dictionary<int, double>();
                    foreach (var permutationElement in columnElement.Elements())
                    {
                        permutationDictionary.Add(permutationElement.Name.XMLToInt(), double.Parse(permutationElement.FirstAttribute.Value));
                    }
                    columnDictionary.Add(columnElement.Name.XMLToInt(), permutationDictionary);
                }
                matrix.Add(rowElement.Name.XMLToInt(), columnDictionary);
            }

            matrixElement = null;
            return matrix;
        }

        private Dictionary<int, Dictionary<int, Dictionary<int, double>>> LoadPartialMatrices(string filepathRoot)
        {
            var matrix = new Dictionary<int, Dictionary<int, Dictionary<int, double>>>();
            for (int j = 0; j < _partialCount; j++)
            {
                var filepath = filepathRoot + "," + j;
                var ijthMatrix = LoadMatrix(filepath);
                matrix = matrix.Union(ijthMatrix).ToDictionary(k => k.Key, v => v.Value);
            }

            return matrix;
        }
    }
}
