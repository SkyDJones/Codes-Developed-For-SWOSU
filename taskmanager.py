from datetime import datetime, timedelta
from apscheduler.schedulers.background import BackgroundScheduler

class Task:
    def __init__(self, title, description, due_date, category, priority):
        self.title = title
        self.description = description
        self.due_date = due_date
        self.category = category
        self.priority = priority
        self.done = False

class TaskManager:
    def __init__(self):
        self.tasks = []
        self.scheduled_reminders = []
        self.scheduler = BackgroundScheduler()
        self.scheduler.start()

    def add_task(self, task, set_reminder=True):
        self.tasks.append(task)
        if set_reminder:
            self.set_reminder(task)

    def display_tasks(self):
        for i, task in enumerate(self.tasks, start=1):
            status = "Done" if task.done else "Pending"
            task_info = (
                f"Title: {task.title}\n"
                f"Description: {task.description}\n"
                f"Due Date: {task.due_date}\n"
                f"Category: {task.category}\n"
                f"Priority: {task.priority}\n"
                f"Status: {status}\n"
            )
            print(f"{i}. {task_info}")

    def mark_task_as_done(self):
        self.display_tasks()
        task_number = int(input("Enter the task number to mark as done: "))
        if 1 <= task_number <= len(self.tasks):
            self.tasks[task_number - 1].done = True
            print("Task marked as done!")
        else:
            print("Invalid task number.")

    def set_reminder(self, task):
        due_date = datetime.strptime(task.due_date, "%Y-%m-%d")
        reminder_date = due_date - timedelta(days=2)

        self.scheduler.add_job(
            self.show_reminder,
            'date',
            run_date=reminder_date,
            args=[task.title]
        )
        self.scheduled_reminders.append((task, reminder_date))

    def show_reminder(self, task_title):
        print(f"Reminder: Task '{task_title}' is due in 2 days!")

    def check_manual_reminders(self):
        print("Manual Reminders:")
        current_date = datetime.now()
        for task, reminder_date in self.scheduled_reminders:
            if not task.done and current_date >= reminder_date:
                print(f"Reminder: Task '{task.title}' is due in 2 days!")

    def edit_task(self):
        self.display_tasks()
        task_number = int(input("Enter the task number to edit: "))
        if 1 <= task_number <= len(self.tasks):
            task_to_edit = self.tasks[task_number - 1]
            print(f"Editing Task: {task_to_edit.title}")
            
            task_to_edit.title = input("Enter new title (press Enter to keep current): ") or task_to_edit.title
            task_to_edit.description = input("Enter new description (press Enter to keep current): ") or task_to_edit.description
            task_to_edit.due_date = input("Enter new due date (YYYY-MM-DD) (press Enter to keep current): ") or task_to_edit.due_date
            task_to_edit.category = input("Enter new category (press Enter to keep current): ") or task_to_edit.category
            task_to_edit.priority = input("Enter new priority (High/Medium/Low) (press Enter to keep current): ") or task_to_edit.priority

            print("Task edited successfully!")
        else:
            print("Invalid task number.")

    def delete_task(self):
        self.display_tasks()
        task_number = int(input("Enter the task number to delete: "))
        if 1 <= task_number <= len(self.tasks):
            deleted_task = self.tasks.pop(task_number - 1)
            print(f"Task '{deleted_task.title}' deleted successfully!")
        else:
            print("Invalid task number.")

    def sort_tasks(self, key="due_date"):
        self.tasks.sort(key=lambda x: getattr(x, key))

    def save_tasks(self, filename="tasks.txt"):
        with open(filename, "w") as file:
            for task in self.tasks:
                file.write(f"{task.title},{task.description},{task.due_date},{task.category},{task.priority},{task.done}\n")

def load_tasks(self, filename="tasks.txt"):
    try:
        with open(filename, "r") as file:
            for line in file:
                task_data = line.strip().split(",")
                new_task = Task(*task_data[:-1])
                new_task.done = bool(task_data[-1])
                self.add_task(new_task, set_reminder=False)
        print("Tasks loaded successfully!")
    except FileNotFoundError:
        print("No previous tasks found.")

if __name__ == "__main__":
    # Initialize the Task Manager
    task_manager = TaskManager()

    # Sample tasks with due dates approaching
    task1 = Task("Complete Project", "Finish the Task Manager project", "2023-12-15", "Work", "High")
    task2 = Task("Study Python", "Read Python documentation", "2023-12-20", "Study", "Medium")
    task_manager.add_task(task1)
    task_manager.add_task(task2)

    print("Welcome To Python Task Manager")

    while True:
        print("\nTask Manager Menu:")
        print("1. View Tasks")
        print("2. Add Task")
        print("3. Mark Task as Done")
        print("4. Check Reminders")
        print("5. Edit Task")
        print("6. Delete Task")
        print("7. Sort Tasks")
        print("8. Exit")


        choice = input("Please Enter your choice (1-8): ")

        if choice == '1':
            task_manager.display_tasks()

        elif choice == '2':
            title = input("Enter task title: ")
            description = input("Enter task description: ")
            due_date = input("Enter due date (YYYY-MM-DD): ")
            category = input("Enter task category: ")
            priority = input("Enter task priority (High/Medium/Low): ")

            new_task = Task(title, description, due_date, category, priority)
            task_manager.add_task(new_task)

        elif choice == '3':
            task_manager.mark_task_as_done()

        elif choice == '4':
            task_manager.check_manual_reminders()

        elif choice == '5':
            task_manager.edit_task()
            
        elif choice == '6':
            task_manager.delete_task()
            
        elif choice == '7':
            sort_key = input("Enter sorting key (due_date/priority/category): ")
            task_manager.sort_tasks(sort_key)
            print("Tasks sorted successfully!")

        elif choice == '8':
            print("Exiting Task Manager. Goodbye!")
            break  # exit loop and end the program

        else:
            print("Invalid choice, please enter a number between 1-8 for your choice")
