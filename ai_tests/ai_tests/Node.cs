using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ai_tests
{
    class Node<T>
    {
        public T value;
        public Node<T> parentNode = null;
        public List<Node<T>> childrenNodes = new List<Node<T>>();
        public Node(T value)
        {
            this.value = value;
        }
        public Node(T value, Node<T> parent)
        {
            this.value = value;
            this.parentNode = parent;
            parent.childrenNodes.Add(this);
        }
        public void AddChild(Node<T> node)
        {
            node.parentNode = this;
            childrenNodes.Add(node);
        }
    }
}
