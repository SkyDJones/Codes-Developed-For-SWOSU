import hashlib
import os

# Dummy user database with hashed passwords and salts
users = {'user1': {'salt': 'randomsalt1', 'hashed_password': '14b8e5c7583903d5d391b7b26ebf7426f8f92b63'},  
         'user2': {'salt': 'randomsalt2', 'hashed_password': '17b8e5c7583903d5d391b7b26ebf7426f8f92b63'}}  

def hash_password(password, salt):
    # Hash the password using SHA-256 algorithm with salt
    hashed_password = hashlib.sha256((password + salt).encode()).hexdigest()
    return hashed_password

def generate_salt():
    # Generate a random salt
    return os.urandom(16).hex()

def login(username, password):
    # Check if username exists in the database
    if username in users:
        stored_salt = users[username]['salt']
        stored_hashed_password = users[username]['hashed_password']
        # Hash the provided password with the stored salt
        hashed_password = hash_password(password, stored_salt)
        # Compare the hashed passwords
        if hashed_password == stored_hashed_password:
            print("Login successful!")
            print(f"Salt: {stored_salt}")
            print(f"Hashed Password with Salt: {stored_hashed_password}")
            return True
        else:
            print("Incorrect password.")
            return False
    else:
        print("Username not found.")
        return False

def create_account():
    username = input("Enter your desired username: ")
    password = input("Enter your desired password: ")
    salt = generate_salt()
    hashed_password = hash_password(password, salt)
    users[username] = {'salt': salt, 'hashed_password': hashed_password}
    print("Account created successfully!")
    return username, password

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

if __name__ == "__main__":
    main()


# Code Created For SWOSU Coding Project, it is one half of the project with another student to make an application with a passwords being hashed for the user login
