import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-button',
  standalone: true,
  imports: [
    CommonModule
  ],
  templateUrl: './button.component.html',
  styleUrl: './button.component.scss'
})
export class ButtonComponent {
  @Output() click: EventEmitter<void> = new EventEmitter<void>();
  @Input() color: 'warn' | 'primary' = 'primary';
}
