# Shape Painter
A simple shape drawing application written with C# / WPF.  
  
### Controls
- If a shape is selected, click to place or click and drag to resize before placing.  
- Shift + drag to create a selection.  
- Ctrl + drag to move the selection.  
- Alt + drag to resize the selection. Only one item may be selected.  
  
### Patterns
Several design patterns were implemented as part of this exercise:  
- **Singleton Pattern:** MainWindow instance.  
- **Command Pattern:** Interactions with the canvas & Undo/Redo functionality.  
- **Visitor Pattern:** A visitor may be used to visit a CanvasObject as either a Group or a Shape.  
- **Strategy Pattern:** Several different click strategies exist to handle the different mouse functionalities described in the Controls paragraph of this document.  
- **Decorator Pattern:** TODO.  