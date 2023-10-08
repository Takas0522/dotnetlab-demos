import { Component, OnInit, inject } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { TodoService } from './services/toso.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {

  todoCtrl = new FormControl('', { validators: Validators.required, nonNullable: true});
  showChangeCtrl = new FormControl(false, { nonNullable: true });

  private service = inject(TodoService);

  ngOnInit(): void {
    this.showChangeCtrl.valueChanges.subscribe(isShowAll => {
      this.service.showTodosChange(isShowAll);
    });
  }

  add() {
    if (this.todoCtrl.invalid) return;
    const addDesc = this.todoCtrl.value;
    this.service.addTodo(addDesc);
    this.todoCtrl.reset();
  }
}
