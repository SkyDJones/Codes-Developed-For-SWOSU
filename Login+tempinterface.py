import tkinter as tk
from tkinter import messagebox
import time
import hashlib
import os

# Global variables
attempts = 3
timer_running = False

# Dummy user database with hashed passwords and salts
users = {'admin': {'salt': 'randomsalt1', 'hashed_password': '14b8e5c7583903d5d391b7b26ebf7426f8f92b63'},  
         'user2': {'salt': 'randomsalt2', 'hashed_password': '17b8e5c7583903d5d391b7b26ebf7426f8f92b63'}}  

def hash_password(password, salt):
    # Hash the password using SHA-256 algorithm with salt
    hashed_password = hashlib.sha256((password + salt).encode()).hexdigest()
    return hashed_password

def generate_salt():
    # Generate a random salt
    return os.urandom(16).hex()

def login(username, password):
    global attempts, timer_running
    
    if timer_running:
        messagebox.showinfo("Please Wait", "You have to wait for the timer to expire before attempting to login again.")
        return False
    
    # Check if username exists in the database
    if username in users:
        stored_salt = users[username]['salt']
        stored_hashed_password = users[username]['hashed_password']
        # Hash the provided password with the stored salt
        hashed_password = hash_password(password, stored_salt)
        # Compare the hashed passwords
        if hashed_password == stored_hashed_password:
            messagebox.showinfo("Login Successful", f"Welcome, {username}!")
            root.destroy()  # Close the window if login is successful
            return True
        else:
            attempts -= 1
            if attempts > 0:
                messagebox.showerror("Login Failed", f"Incorrect username or password. {attempts} attempts left.")
            else:
                messagebox.showerror("Login Failed", "Incorrect username or password. No attempts left. Please wait for 30 seconds.")
                # Start timer
                start_timer()
            return False
    else:
        messagebox.showerror("Login Failed", "Username not found.")
        return False

def create_account():
    username = input("Enter your desired username: ")
    password = input("Enter your desired password: ")
    salt = generate_salt()
    hashed_password = hash_password(password, salt)
    users[username] = {'salt': salt, 'hashed_password': hashed_password}
    print("Account created successfully!")
    return username, password

def start_timer():
    global timer_running
    timer_running = True
    for i in range(30, 0, -1):
        time.sleep(1)
    timer_running = False
    global attempts
    attempts = 3

def main():
    print("Welcome to the login page.")
    while True:
        username = input("Enter your username: ")
        password = input("Enter your password: ")
        if login(username, password):
            break
        else:
            choice = input("Do you want to create a new account? (yes/no): ").lower()
            if choice == 'yes':
                new_username, new_password = create_account()
                print("Please log in with your new account.")
                continue

# Create main window
root = tk.Tk()
root.title("Login")

# Create username label and entry
username_label = tk.Label(root, text="Username:")
username_label.grid(row=0, column=0, padx=5, pady=5, sticky=tk.E)

username_entry = tk.Entry(root)
username_entry.grid(row=0, column=1, padx=5, pady=5)

# Create password label and entry
password_label = tk.Label(root, text="Password:")
password_label.grid(row=1, column=0, padx=5, pady=5, sticky=tk.E)

password_entry = tk.Entry(root, show="*")
password_entry.grid(row=1, column=1, padx=5, pady=5)

# Create login button
login_button = tk.Button(root, text="Login", command=lambda: login(username_entry.get(), password_entry.get()))
login_button.grid(row=2, column=0, columnspan=2, padx=5, pady=5)

# Run the main event loop
root.mainloop()

# Start the main function after the tkinter window closes
if __name__ == "__main__":
    main()
