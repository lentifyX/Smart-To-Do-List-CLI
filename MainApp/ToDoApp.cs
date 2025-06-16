using System;
using System.Collections.Generic;
using System.Linq;
using SmartToDoList.Models;
using SmartToDoList.Structures;

namespace SmartToDoList
{
    public class ToDoApp
    {
        private List<TaskItem> tasks = new();
        private MyPriorityQueue priorityQueue = new();
        private MyStack<Action> undoStack = new();
        private int nextId = 1;

        public void Run()
        {
            string input;
            Console.WriteLine("Smart To-Do List CLI");
            Console.WriteLine("Ketik 'help' untuk melihat daftar perintah.");

            while (true)
            {
                Console.Write("\n> ");
                input = Console.ReadLine() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(input)) continue;

                var args = input.Split(' ', 2);
                var command = args[0];
                var value = args.Length > 1 ? args[1] : string.Empty;

                switch (command)
                {
                    case "tambah": TambahTugas(value); break;
                    case "lihat": LihatTugas(value); break;
                    case "selesai": SelesaikanTugas(value); break;
                    case "hapus": HapusTugas(value); break;
                    case "edit": EditTugas(value); break;
                    case "cari": CariTugas(value); break;
                    case "prioritas": UbahPrioritas(value); break;
                    case "undo": Undo(); break;
                    case "help": ShowHelp(); break;
                    case "out": return;
                    default: Console.WriteLine("Perintah tidak dikenal."); break;
                }
            }
        }

        private void TambahTugas(string value)
        {
            var parts = value.Split(',');

            if (parts.Length < 2)
            {
                Console.WriteLine("? Format salah. Format: tambah nama_tugas,prioritas");
                return;
            }

            string nama = parts[0].Trim();
            string prio = parts[1].Trim();

            if (string.IsNullOrWhiteSpace(nama))
            {
                Console.WriteLine("Nama tugas tidak boleh kosong.");
                return;
            }

            if (string.IsNullOrWhiteSpace(prio))
            {
                Console.WriteLine("Prioritas tidak boleh kosong.");
                return;
            }

            if (!int.TryParse(prio, out int prioritas) || prioritas < 1)
            {
                Console.WriteLine("Prioritas harus berupa angka bulat positif.");
                return;
            }

            var task = new TaskItem
            {
                Id = nextId++,
                Name = nama,
                Priority = prioritas
            };

            tasks.Add(task);
            priorityQueue.Enqueue(task);
            undoStack.Push(() => { tasks.Remove(task); priorityQueue.Remove(task); });

            Console.WriteLine($"Tugas {nama} dengan prioritas {prioritas} berhasil ditambahkan.");
        }


        private void LihatTugas(string value)
        {
            var filtered = tasks;
            if (value == "-selesai")
                filtered = tasks.Where(t => t.IsCompleted).ToList();
            else
                filtered = tasks.Where(t => !t.IsCompleted).ToList();

            var sorted = filtered.OrderBy(t => t.Priority);
            foreach (var task in sorted)
                Console.WriteLine(task);
        }

        private void SelesaikanTugas(string value)
        {
            TaskItem? task = null; 
            if (int.TryParse(value, out int id))
                task = tasks.FirstOrDefault(t => t.Id == id);
            else
                task = priorityQueue.Peek();

            if (task != null && !task.IsCompleted)
            {
                task.IsCompleted = true;
                undoStack.Push(() => task.IsCompleted = false);
                Console.WriteLine("Tugas diselesaikan.");
            }
            else
            {
                Console.WriteLine("Tugas tidak ditemukan atau sudah selesai.");
            }
        }


        private void HapusTugas(string value)
        {
            if (int.TryParse(value, out int id))
            {
                var task = tasks.FirstOrDefault(t => t.Id == id);
                if (task != null)
                {
                    tasks.Remove(task);
                    priorityQueue.Remove(task);
                    undoStack.Push(() => { tasks.Add(task); priorityQueue.Enqueue(task); });
                    Console.WriteLine("Tugas dihapus.");
                }
                else Console.WriteLine("Tugas tidak ditemukan.");
            }
        }

        private void EditTugas(string value)
        {
            var parts = value.Split(',');
            if (parts.Length < 3) { Console.WriteLine("Format: edit id,nama_baru,prioritas_baru"); return; }

            if (int.TryParse(parts[0], out int id))
            {
                var task = tasks.FirstOrDefault(t => t.Id == id);
                if (task != null)
                {
                    var oldName = task.Name;
                    var oldPriority = task.Priority;
                    task.Name = parts[1].Trim();
                    task.Priority = int.Parse(parts[2].Trim());
                    priorityQueue.Update(task);
                    undoStack.Push(() => { task.Name = oldName; task.Priority = oldPriority; priorityQueue.Update(task); });
                    Console.WriteLine("Tugas diubah.");
                }


            }
        }

        private void CariTugas(string value)
        {
            var result = tasks.Where(t => t.Id.ToString() == value || t.Name.Contains(value, StringComparison.OrdinalIgnoreCase));
            foreach (var task in result)
                Console.WriteLine(task);
        }

        private void UbahPrioritas(string value)
        {
            var parts = value.Split(',');
            if (parts.Length < 2) { Console.WriteLine("Format: prioritas id,prioritas_baru"); return; }
            if (int.TryParse(parts[0], out int id) && int.TryParse(parts[1], out int newPriority))
            {
                var task = tasks.FirstOrDefault(t => t.Id == id);
                if (task != null)
                {
                    var oldPriority = task.Priority;
                    task.Priority = newPriority;
                    priorityQueue.Update(task);
                    undoStack.Push(() => { task.Priority = oldPriority; priorityQueue.Update(task); });
                    Console.WriteLine("Prioritas diperbarui.");
                }
            }
        }

        private void Undo()
        {
            if (undoStack.Count > 0)
            {
                var action = undoStack.Pop();
                action();
                Console.WriteLine("Aksi terakhir dibatalkan.");
            }
            else Console.WriteLine("Tidak ada aksi untuk dibatalkan.");
        }

        private void ShowHelp()
        {
            Console.WriteLine("Perintah yang tersedia:");
            Console.WriteLine("tambah nama,prioritas");
            Console.WriteLine("lihat [-selesai]");
            Console.WriteLine("selesai [id]");
            Console.WriteLine("hapus id");
            Console.WriteLine("edit id,nama_baru,prioritas_baru");
            Console.WriteLine("cari id/keyword");
            Console.WriteLine("prioritas id,prioritas_baru");
            Console.WriteLine("undo");
            Console.WriteLine("out");
        }
    }
}
