# Shape Painter
A simple shape drawing application written with C# / WPF.  
  
### Controls
- If a shape is selected, click to place or click and drag to resize before placing.  
- Shift + drag to create a selection.  
- Ctrl + drag to move the selected items.  
- Alt + drag to resize the selected item. Only one item may be selected.  
- In the group menu, click to select or deselect a group of shape.
- In the group menu, drag and drop one or more objects to change the hierarchy.
- In the group menu, ctrl + click to select or deselect a group or shape while leaving the existing selection unchanged.
- In the group menu, X + click to open the Canvas Object Settings Manager.
- Press Escape to deselect all selected items.
- Press Delete to delete all selected items.
  
### Patterns
Several design patterns were implemented as part of this exercise:  
- **Singleton Pattern:** MainWindow instance.  
- **Command Pattern:** Interactions with the canvas & Undo/Redo functionality.  
- **Visitor Pattern:** A visitor may be used to visit a CanvasObject as either a Group or a Shape.  
- **Strategy Pattern:** Several different click strategies exist to handle the different mouse functionalities described in the Controls paragraph of this document.  
- **Decorator Pattern:** TODO.  
