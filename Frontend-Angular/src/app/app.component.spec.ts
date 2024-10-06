import { ComponentFixture, fakeAsync, TestBed } from '@angular/core/testing';
import { AppComponent } from './app.component';
import {
  HttpClient,
  HttpClientModule,
  HttpErrorResponse,
} from '@angular/common/http';
import { ToastrModule, ToastrService } from 'ngx-toastr';
import { TodoItemService } from './todoItem.service';
import { of, throwError } from 'rxjs';
import { TodoItem } from './models/todoItem';
import { FormsModule } from '@angular/forms';

describe('appComponent', () => {
  let fixture: ComponentFixture<AppComponent>;
  let component: AppComponent;
  let element: HTMLElement;
  let toDoItemService: TodoItemService;
  let toastrService: ToastrService;

  let initialToDoItems = [
    { id: 'A', description: 'Describes A', isCompleted: false },
  ] as TodoItem[];

  class FakeToDoItemService {
    get() {
      return of(initialToDoItems);
    }
    add(toDoItem: TodoItem) {
      return of(toDoItem);
    }
  }

  class FakeToastrService {
    info(a: string, b: string) {}
    success(a: string, b: string) {}
    error(a: string, b: string) {}
  }

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [FormsModule, HttpClientModule, ToastrModule.forRoot()],
      declarations: [AppComponent],
      providers: [
        { provide: TodoItemService, useClass: FakeToDoItemService },
        { provide: ToastrService, useClass: FakeToastrService },
        HttpClient,
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(AppComponent);
    component = fixture.componentInstance;
    element = fixture.nativeElement;
    toDoItemService = TestBed.inject(TodoItemService);
    toastrService = TestBed.inject(ToastrService);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
    spyOn(toDoItemService, 'get').and.callThrough();
    spyOn(toastrService, 'info').and.callThrough();
    component.ngOnInit();
    expect(toDoItemService.get).toHaveBeenCalled();
    expect(toastrService.info).toHaveBeenCalledTimes(0);
    expect(component.items).toEqual(initialToDoItems);
  });

  it('clicking Refresh button should cause toDoItemService.get and toastrService.info to be called', () => {
    const result = [...initialToDoItems].concat([
      { id: 'B', description: 'Describes B', isCompleted: false },
    ] as TodoItem[]);
    spyOn(toDoItemService, 'get').and.callFake(() => {
      return of(result);
    });
    spyOn(toastrService, 'info').and.callThrough();
    const buttonDebugElement = fixture.debugElement.query(
      (x) => x.nativeNode.innerText == 'Refresh'
    );
    expect(buttonDebugElement).toBeTruthy();
    const buttonNativeElement = buttonDebugElement.nativeElement;
    buttonNativeElement.click();
    expect(toDoItemService.get).toHaveBeenCalled();
    expect(toastrService.info).toHaveBeenCalled();
    expect(component.items).toEqual(result);
  });

  it('clicking Refresh button should cause toDoItemService.get (with a forced error) and toastrService.error to be called', fakeAsync(() => {
    spyOn(toDoItemService, 'get').and.returnValue(
      throwError(() => {
        return { error: 'Some 400 message' } as HttpErrorResponse;
      })
    );
    spyOn(toastrService, 'error').and.callThrough();
    const buttonDebugElement = fixture.debugElement.query(
      (x) => x.nativeNode.innerText == 'Refresh'
    );
    expect(buttonDebugElement).toBeTruthy();
    const buttonNativeElement = buttonDebugElement.nativeElement;
    buttonNativeElement.click();
    expect(toDoItemService.get).toHaveBeenCalled();
    expect(toastrService.error).toHaveBeenCalledWith(
      'Some 400 message',
      'Refresh'
    );
    expect(component.items).toEqual(initialToDoItems);
  }));

  it('inputing into Description and clicking Add Item button should cause toDoItemService.add and toastrService.success to be called', () => {
    const descriptionDebugElement = fixture.debugElement.query(
      (x) => x.nativeNode.placeholder == 'Enter description...'
    );
    const descriptionNativeElement = descriptionDebugElement.nativeElement;
    descriptionNativeElement.value = 'Describes B';
    descriptionNativeElement.dispatchEvent(new Event('input'));
    const resultAddToDoItem = {
      id: 'B',
      description: descriptionNativeElement.value,
      isCompleted: false,
    } as TodoItem;
    const result = [...initialToDoItems].concat([resultAddToDoItem]);
    spyOn(toDoItemService, 'add').and.callFake(() => {
      return of(resultAddToDoItem);
    });
    spyOn(toastrService, 'success').and.callThrough();
    const addDebugElement = fixture.debugElement.query(
      (x) => x.name == 'button' && x.nativeNode.innerText == 'Add Item'
    );
    expect(addDebugElement).toBeTruthy();
    const buttonNativeElement = addDebugElement.nativeElement;
    buttonNativeElement.click();
    expect(toDoItemService.add).toHaveBeenCalled();
    expect(toastrService.success).toHaveBeenCalled();
    expect(component.items).toEqual(result);
  });

  /* Most complicated example done, cut and paste away as appropriate ;) */
});
