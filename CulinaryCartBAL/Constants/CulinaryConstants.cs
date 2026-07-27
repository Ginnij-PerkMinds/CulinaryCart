namespace CulinaryCart.CulinaryCartBAL.Constants
{
    public static class CulinaryCartConstants
    {
        public static class Status
        {
            public const string InCart = "InCart";
            public const string CheckedOut = "CheckedOut";
        }

        public static class Messages
        {
            public const string ItemAdded = "Item added";
            public const string ItemUpdated = "Item updated";
            public const string ItemRemoved = "Item removed";
            public const string OrderPlaced = "Order placed";
            public const string CartisEmpty = "Cart is empty";

            public const string CheckoutSuccessful = "Checkout successful. Stock updated.";

            public const string AlreadyInDB = "Already in DB";

            public const string UserIdClaimMissing = "User ID claim is missing in the token.";

            public const string CategoryAdded = "Category added successfully.";
            public const string CategoryUpdated = "Category updated successfully.";
            public const string CategoryDeleted = "Category deleted successfully.";
            public const string CategoryNotFound = "Category not found";
            public const string CategoryNameRequired = "Category name is required.";
            public const string CategoryUpdateFailed = "Category update failed.";
            public const string CategoryDeleteFailed = "Category delete failed.";
            public const string CategoryUpdateNameRequired = "New category name is required.";
            public const string InvalidCategoryName = "Invalid category name.";

            public const string DietaryPreferenceAdded = "Dietary preference added successfully.";
            public const string DietaryPreferenceUpdated = "Dietary preference updated successfully.";
            public const string DietaryPreferenceDeleted = "Dietary preference deleted successfully.";
            public const string DietaryPreferenceNotFound = "Dietary preference not found";
            public const string DietaryPreferenceNameRequired = "Dietary preference name is required.";
            public const string DietaryPreferenceUpdateFailed = "Dietary preference update failed.";
            public const string DietaryPreferenceDeleteFailed = "Dietary preference delete failed.";
            public const string DietaryPreferenceUpdateNameRequired = "New dietary preference name is required.";
            public const string InvalidDietaryPreferenceName = "Invalid dietary preference name.";

            public const string NoMenuItemsAvailable = "No menu items available.";
            public const string MenuItemAdded = "Menu item added successfully.";
            public const string MenuItemUpdated = "Menu item updated successfully.";
            public const string MenuItemDeleted = "Menu item deleted successfully.";
            public const string MenuItemNotFound = "Menu item not found";
            public const string MenuItemNameRequired = "Menu item name is required.";
            public const string MenuItemPriceRequired = "Menu item price is required.";
            public const string MenuItemUpdateFailed = "Menu item update failed.";
            public const string MenuItemDeleteFailed = "Menu item delete failed.";
            public const string MenuItemUpdateNameRequired = "New menu item name is required.";
            public const string MenuItemUpdatePriceRequired = "New menu item price is required.";
            public const string MenuItemUpdateCategoryRequired = "New menu item category is required.";
            public const string MenuItemUpdateDietaryPreferenceRequired = "New menu item dietary preference is required.";
            public const string MenuItemUpdateImageRequired = "New menu item image is required.";
            public const string MenuItemUpdateOffersRequired = "New menu item offers is required.";
            public const string MenuItemUpdateStockRequired = "New menu item stock status is required.";
            public const string MenuItemUpdateInStockRequired = "New menu item in-stock status is required.";
            public const string MenuItemUpdateOutOfStockRequired = "New menu item out-of-stock status is required.";
            public const string MenuItemUpdateInvalidCategory = "Invalid category name.";
            public const string MenuItemUpdateInvalidDietaryPreference = "Invalid dietary preference name.";
            public const string StockUpdateFailed = "Stock update failed.";
            public const string StockUpdateSuccessful = "Stock updated successfully.";

            public const string UserAlreadyExists = "User already exists";
            public const string InvalidEmailOrPassword = "Invalid email or password";
            public const string PasswordRequirements = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one digit, and one special character.";
            public const string UserNotFound = "User not found";
            public const string LogoutSuccessful = "Logout successful";
            public const string LogoutFailed = "Logout failed";
            public const string LoginSuccessful = "Login successful";

            public const string FlagsUpdated = "Flags updated successfully";
            public const string UserAdded = "User added successfully";
            public const string UserUpdated = "User updated successfully";
            public const string UserDeleted = "User deleted successfully";
            public const string UserExists = "User already exists";
            public const string IncorrectOldPassword = "Incorrect old password";
            public const string UserUpdateFailed = "User update failed";
            public const string UserDeleteFailed = "User delete failed";
            public const string UserUpdateInvalidRole = "Invalid user role.";
            public const string UserUpdateInvalidEmail = "Invalid user email.";
            public const string UserUpdateInvalidPassword = "Invalid user password.";
            public const string UserUpdateInvalidProfileImage = "Invalid user profile image.";
            public const string UserUpdateInvalidName = "Invalid user name.";
            public const string PasswordUpdateSuccessful = "Password updated successfully";
            public const string PasswordUpdateFailed = "Password update failed";
            public const string InvalidToken = "Invalid token";

        }
    }
}
