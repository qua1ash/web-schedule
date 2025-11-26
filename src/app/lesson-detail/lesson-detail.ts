import { Component, Inject } from '@angular/core';
import { MAT_BOTTOM_SHEET_DATA } from '@angular/material/bottom-sheet';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';

export interface LessonData {
  order: number;
  date: Date;
  start: string;
  end: string;
  title: string;
  teacher: string;
  room: string;
  isAdmin?: boolean;
}

@Component({
  selector: 'app-lesson-detail',
  imports: [CommonModule, FormsModule, MatButtonModule],
  templateUrl: './lesson-detail.html',
  styleUrls: ['./lesson-detail.scss']
})
export class LessonDetailComponent {
  homework: string = '';

  constructor(@Inject(MAT_BOTTOM_SHEET_DATA) public data: LessonData) {
    this.loadHomework();
  }

  private loadHomework(): void {
    const dateStr = this.data.date.toISOString().split('T')[0]; // YYYY-MM-DD format
    const key = `homework_${dateStr}`;
    const saved = localStorage.getItem(key);
    this.homework = saved || '';
  }

  saveHomework(): void {
    const dateStr = this.data.date.toISOString().split('T')[0]; // YYYY-MM-DD format
    const key = `homework_${dateStr}`;
    localStorage.setItem(key, this.homework);
    // Заглушка для backend: здесь можно добавить вызов API
    console.log('Homework saved:', this.homework);
  }
}
