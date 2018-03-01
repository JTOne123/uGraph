﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace uGraph
{
    /// <summary>
    /// Generic Graph implementarion based on Adjacency list 
    /// </summary>
    /// <typeparam name="TVertex"></typeparam>
    /// <typeparam name="TEdge"></typeparam>
    public class Graph<TVertex, TEdge> : IEnumerable<Vertex<TVertex, TEdge>>
    {
        private List<Vertex<TVertex, TEdge>> vertices;

        private readonly HashSet<Guid> verticesSet;

        public IReadOnlyList<Vertex<TVertex, TEdge>> Vertices
        {
            get
            {
                return vertices.AsReadOnly();
            }
        }

        public int VertexCount { get { return verticesSet.Count; } }

        public int EdgeCount { get; set; }

        public Graph()
        {
            vertices = new List<Vertex<TVertex, TEdge>>();
            verticesSet = new HashSet<Guid>();
        }

        public void AddVertex(Vertex<TVertex, TEdge> vertex)
        {
            vertices.Add(vertex);
            verticesSet.Add(vertex.Id);
        }

        public Vertex<TVertex, TEdge> AddVertex(TVertex info)
        {
            var vertex = new Vertex<TVertex, TEdge> { Info = info };

            AddVertex(vertex);

            return vertex;
        }

        public Edge<TVertex, TEdge> AddEdge(Vertex<TVertex, TEdge> origin, Vertex<TVertex, TEdge> destination, TEdge info)
        {
            var edge = new Edge<TVertex, TEdge>(info);

            edge.Origin = origin;
            edge.Destination = destination;

            origin.Edges.Add(edge);

            EdgeCount++;

            return edge;
        }

        public Edge<TVertex, TEdge> AddEdge<T>(T origin, T destination, TEdge info) where T : TVertex, IEquatable<TVertex>
        {
            var originVertex = FirstOrDefault(v => ((IEquatable<T>)(v)).Equals(origin));

            if (originVertex == null)
                throw new ArgumentNullException(nameof(originVertex));

            var destinationVertex = FirstOrDefault(v => ((IEquatable<T>)(v)).Equals(destination));

            if (destinationVertex == null)
                throw new ArgumentNullException(nameof(destinationVertex));

            return AddEdge(originVertex, destinationVertex, info);
        }

        public Vertex<TVertex, TEdge> FirstOrDefault(Func<TVertex, bool> predicate)
        {
            return vertices.FirstOrDefault(v => predicate(v.Info));
        }

        public bool Contains<T>(T value) where T : TVertex, IEquatable<TVertex>
        {
            return vertices.Any(v => v.Info.Equals(value));
        }

        public bool Contains(Vertex<TVertex, TEdge> vertex)
        {
            return vertices.Any(v => v.Equals(vertex));
        }

        public IEnumerator<Vertex<TVertex, TEdge>> GetEnumerator()
        {
            return vertices.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return vertices.GetEnumerator();
        }

        private void ClearVisitedVertices()
        {
            foreach (var vertex in vertices)
            {
                vertex.Visited = false;
            }
        }

        /// <summary>
        /// Depth-first traversal
        /// </summary>
        public void DFT(Vertex<TVertex, TEdge> initialVertex, Action<Vertex<TVertex, TEdge>> action)
        {
            if (initialVertex == null)
                throw new ArgumentNullException(nameof(initialVertex));

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            Stack<Vertex<TVertex, TEdge>> stack = new Stack<Vertex<TVertex, TEdge>>();

            ClearVisitedVertices();

            stack.Push(initialVertex);

            while (stack.Count > 0)
            {
                var currentVertex = stack.Pop();

                if (!currentVertex.Visited)
                {
                    currentVertex.Visited = true;

                    action(currentVertex);

                    foreach (var edge in currentVertex.Edges)
                    {
                        stack.Push(edge.Destination);
                    }
                }
            }
        }
    }
}
