import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TodoService } from '../services/todo.service';
import { Todo } from '../models/todo.model';

@Component({
    selector: 'app-todo',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './todo.html',
    styleUrl: './todo.scss'
})
export class TodoComponent {
    private todoService = inject(TodoService);

    todos: Todo[] = [];

    newTitle = '';

    async ngOnInit() {
        this.todos = await this.todoService.getTodos();
    }

    async addTodo() {
        if (!this.newTitle.trim()) return;
        const newTodo = await this.todoService.addTodo(this.newTitle);
        this.todos.push(newTodo);
        this.newTitle = '';
    }

    async toggleComplete(todo: Todo) {
        todo.isCompleted = !todo.isCompleted;
        await this.todoService.updateTodo(todo);
    }

    async deleteTodo(todo: Todo) {
        await this.todoService.deleteTodo(todo.id);
        this.todos = this.todos.filter(t => t.id !== todo.id);
    }
}