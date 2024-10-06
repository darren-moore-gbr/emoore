import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TodoItem } from './models/todoItem';
import { Observable } from 'rxjs';

@Injectable()
export class TodoItemService {
  private url = 'https://localhost:44397/api';

  public constructor(private httpClient: HttpClient) {}

  public get(): Observable<TodoItem[]> {
    return this.httpClient.get<TodoItem[]>(`${this.url}/todoitems`);
  }

  public add(toDoItem: TodoItem): Observable<TodoItem> {
    return this.httpClient.post<TodoItem>(`${this.url}/todoitems`, toDoItem);
  }

  public put(toDoItem: TodoItem): Observable<TodoItem> {
    return this.httpClient.put<TodoItem>(`${this.url}/todoitems/${toDoItem.id}`, toDoItem);
  }
}
