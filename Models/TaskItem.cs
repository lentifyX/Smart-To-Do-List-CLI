using System;

namespace SmartToDoList.Models
{
    public class TaskItem : IComparable<TaskItem>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Priority { get; set; }
        public bool IsCompleted { get; set; } = false;

        public int CompareTo(TaskItem? other)
        {
            if (other == null) return 1; 
            return other.Priority.CompareTo(this.Priority);
        }

        public override string ToString()
        {
            return $"[ID: {Id}] {Name} (Prioritas: {Priority}) - {(IsCompleted ? "Selesai" : "Belum Selesai")}"; 
        }
     }
}

