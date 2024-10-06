import { Component, OnInit } from '@angular/core';
import { TodoItem } from './models/todoItem';
import { ToastrService } from 'ngx-toastr';
import { TodoItemService } from './todoItem.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit {
  items: TodoItem[] = [];
  description: string = '';

  public constructor(private toDoItemService: TodoItemService, private toastrService: ToastrService) {}

  public ngOnInit() {
    this.getItems('Load');
  }

  getItems(operation:string = 'Refresh') {
    this.toDoItemService.get().subscribe({
      next: result => {
        this.items = result;
        if (operation == 'Refresh') {
          this.toastrService.info('Completed successfully', operation);
        }
      },
      error: err => {
        this.toastrService.error(err.error ?? 'Network error', operation);
      }
    });
  }

  handleAdd() {
    const toDoItem = {
      description: this.description
    } as TodoItem;
    this.toDoItemService.add(toDoItem).subscribe({
      next: result => {
        this.items = [...this.items, result];
        this.handleClear();
        this.toastrService.success('Completed successfully', 'Add Item');
      },
      error: err => {
        this.toastrService.error(err.error ?? 'Network error', 'Add Item');
      }
    });
  }

  handleClear() {
    this.description = '';
  }

  handleMarkAsComplete(toDoItem: TodoItem) {
    toDoItem.isCompleted = true;
    this.toDoItemService.put(toDoItem).subscribe({
      next: () => {
        this.items = [...this.items].filter(x => x.id != toDoItem.id);
        this.toastrService.success('Completed successfully', 'Mark as completed');
      },
      error: err => {
        this.toastrService.error(err.error ?? 'Network error', 'Mark as completed');
      }
    });
  }
}
