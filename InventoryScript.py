# Inventory Report Function
def inv_report():
    print("Inventory Report:")
    if not inventory:
        print("The inventory is currently empty.")
    else:
        # Sort inventory by category
        sorted_inventory = sorted(inventory.items(), key=lambda x: x[1][0])

        # Print Item, Category, and Quantity
        for item, (category, quantity) in sorted_inventory:
            print(f"{item} (Category: {category}): {quantity}")

# Add Function
def addinv():
    item = input("Enter item name: ")
    category = input("Enter item category: ")
    while True:
        try:
            quantity = int(input("Enter quantity to add: "))
            if quantity < 0:
                print("Please enter a non-negative quantity.")
            else:
                break
        except ValueError:
            print("Invalid input. Please enter a valid integer.")

    if item in inventory:
        current_category, current_quantity = inventory[item]
        inventory[item] = (current_category, current_quantity + quantity)
    else:
        inventory[item] = (category, quantity)

    print(f"{quantity} {item}(s) (Category: {category}) added to inventory.")

# Remove Function
def removeinv():
    item = input("Enter item name to remove: ")
    if item in inventory:
        current_quantity = inventory[item][1]
        print(f"Current quantity of {item} in inventory: {current_quantity}")

        while True:
            print("Options:")
            print("Remove a specific quantity")
            print("Remove all")
            print("Cancel")
            remove_choice = input("Please select an option:")

            if remove_choice == 'Remove a specific quantity':
                while True:
                    try:
                        quantity_to_remove = int(input(f"Enter the quantity of {item} to remove: "))
                        if quantity_to_remove < 0:
                            print("Please enter a non-negative quantity.")
                        else:
                            break
                    except ValueError:
                        print("Invalid input. Please enter a valid integer.")

                if quantity_to_remove <= current_quantity and quantity_to_remove > 0:
                    inventory[item] = (inventory[item][0], current_quantity - quantity_to_remove)
                    print(f"{quantity_to_remove} {item}(s) removed from inventory.")
                    break
                elif quantity_to_remove <= 0:
                    print("Please enter a valid positive quantity.")
                else:
                    print(f"Not enough {item} in inventory.")
            elif remove_choice == 'Remove all':
                removed_quantity = current_quantity
                inventory.pop(item)
                print(f"All {item} (Category: {inventory[item][0]}) removed from inventory. Amount removed: {removed_quantity}")
                break
            elif remove_choice == 'Cancel':
                break
            else:
                print("Invalid option. Please try again.")

    else:
        print(f"{item} not found in inventory.")

# Update Function
def updateinv():
    item = input("Enter item name to update: ")
    if item in inventory:
        print("Options:")
        print("Update Item Quantity")
        print("Update Category")
        print("Update Item Name")
        print("Update All")
        print("Cancel Update")
        update_option = input("Please Select Option:")

        if update_option == "Update Item Quantity":
            while True:
                try:
                    new_quantity = int(input("Enter the new quantity: "))
                    if new_quantity < 0:
                        print("Please enter a non-negative quantity.")
                    else:
                        break
                except ValueError:
                    print("Invalid input. Please enter a valid integer.")

            category, _ = inventory[item]
            inventory[item] = (category, new_quantity)
            print(f"Quantity of {item} updated to {new_quantity}.")

        elif update_option == "Update Category":
            new_category = input("Enter New Category Name: ")
            inventory[item] = (new_category, inventory[item][1])
            print(f"Category of {item} updated to {new_category}.")

        elif update_option == "Update Item Name":
            new_item_name = input("Enter New Item Name: ")
            inventory[new_item_name] = inventory.pop(item)
            print(f"{item} updated to {new_item_name}.")

        elif update_option == "Update All":
            new_item_name = input("Enter the new item name: ")
            new_category = input("Enter the new category: ")
            while True:
                try:
                    new_quantity = int(input("Enter the new quantity: "))
                    if new_quantity < 0:
                        print("Please enter a non-negative quantity.")
                    else:
                        break
                except ValueError:
                    print("Invalid input. Please enter a valid integer.")

            inventory[new_item_name] = (new_category, new_quantity)
            inventory.pop(item)
            print(f"{item} updated to {new_item_name} with category {new_category} and quantity {new_quantity}.")

        elif update_option == "Cancel Update":
            print("Update operation cancelled.")

        else:
            print("Invalid option. Please try again.")

    else:
        print(f"{item} not found in inventory.")

# Search Function
def searchinv():
    while True:
        print("Search Options:")
        print("Search by Item(s)")
        print("Search by Category(s)")
        print("Go Back to Main Menu")
        search_choice = input("Please select search option:")

        if search_choice == 'Search by Item(s)':
            items_to_search = input("Enter item(s) to search (comma-separated): ").split(',')
            items_to_search = [item.strip() for item in items_to_search]
            for item_name in items_to_search:
                if item_name in inventory:
                    category, quantity = inventory[item_name]
                    print(f"{item_name} (Category: {category}) found in inventory with a quantity of {quantity}.")
                else:
                    print(f"{item_name} not found in inventory.")
        elif search_choice == 'Search by Category(s)':
            categories_to_search = input("Enter category(s) to search (comma-separated): ").split(',')
            categories_to_search = [category.strip().lower() for category in categories_to_search]
            found = False
            for item, details in inventory.items():
                category, quantity = details
                if category.lower() in categories_to_search:
                    found = True
                    print(f"{item} (Category: {category}): {quantity}")
            if not found:
                print(f"No items found in the specified categories.")
        elif search_choice == 'Go Back to Main Menu':
            break  # Go back to the main program
        else:
            print("Invalid search option. Please try again.")

# Main program loop
while True:
    print("---------------------------------------------")
    print("-       Welcome To Inventory Manager        -")
    print("---------------------------------------------")

    choice = input("Please Enter Options or Review Options:")
    
    if choice == 'Review Options':
        print("Options:")
        print("Inventory Report")
        print("Add To Inventory")
        print("Remove From Inventory")
        print("Update Inventory")
        print("Search Inventory")
        print("Exit")
    elif choice == 'Inventory Report':
        inv_report()
    elif choice == 'Add To Inventory':
        addinv()
    elif choice == 'Remove From Inventory':
        removeinv()
    elif choice == 'Update Inventory':
        updateinv()
    elif choice == 'Search Inventory':
        searchinv()
    elif choice == 'Exit':
        break  # Exit the loop and end the program
    else:
        print("Invalid Option, Please Type a Valid Option(ex: Inventory Report, Add To Inventory, etc ")
