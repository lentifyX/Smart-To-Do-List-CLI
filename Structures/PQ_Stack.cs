using System;
using System.Collections.Generic;
using SmartToDoList.Models;

namespace SmartToDoList.Structures
{
    public class MyPriorityQueue
    {
        private TaskItem[] heap = new TaskItem[100];
        private int size = 0;

        public void Enqueue(TaskItem item)
        {
            if (size >= heap.Length)
                Resize();

            heap[size] = item;
            int i = size++;
            while (i > 0 && heap[i].Priority < heap[(i - 1) / 2].Priority)
            {
                Swap(i, (i - 1) / 2);
                i = (i - 1) / 2;
            }
        }

        public TaskItem Peek()
        {
            return size > 0 ? heap[0] : new TaskItem { Id = 0, Name = string.Empty, Priority = int.MaxValue, IsCompleted = false };
        }


        public TaskItem Dequeue()
        {
            if (size == 0)
                return new TaskItem { Id = 0, Name = string.Empty, Priority = int.MaxValue, IsCompleted = false };
            TaskItem top = heap[0];
            heap[0] = heap[--size];
            Heapify(0);
            return top;
        }


        public void Remove(TaskItem target)
        {
            for (int i = 0; i < size; i++)
            {
                if (heap[i].Id == target.Id)
                {
                    heap[i] = heap[--size];
                    Heapify(i);
                    break;
                }
            }
        }

        public void Update(TaskItem task)
        {
            for (int i = size / 2 - 1; i >= 0; i--)
                Heapify(i);
        }

        private void Heapify(int i)
        {
            int smallest = i;
            int left = 2 * i + 1;
            int right = 2 * i + 2;

            if (left < size && heap[left].Priority < heap[smallest].Priority)
                smallest = left;
            if (right < size && heap[right].Priority < heap[smallest].Priority)
                smallest = right;

            if (smallest != i)
            {
                Swap(i, smallest);
                Heapify(smallest);
            }

        }

        private void Swap(int a, int b)
        {
            TaskItem temp = heap[a];
            heap[a] = heap[b];
            heap[b] = temp;
        }

        private void Resize()
        {
            TaskItem[] newHeap = new TaskItem[heap.Length * 2];
            for (int i = 0; i < heap.Length; i++)
                newHeap[i] = heap[i];
            heap = newHeap;
        }
    }

    public class MyStack<T>
    {
        private List<T> items = new();

        public void Push(T item) => items.Add(item);

        public T Pop()
        {
            if (items.Count == 0) throw new InvalidOperationException("kosong.");
            var item = items[^1];
            items.RemoveAt(items.Count - 1);
            return item;
        }

        public int Count => items.Count;
    }
}
