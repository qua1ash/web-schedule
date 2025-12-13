import { Component, Inject } from '@angular/core';
import { MAT_BOTTOM_SHEET_DATA } from '@angular/material/bottom-sheet';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { HomeworkService } from '../services/homework.service';

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
  isLoading: boolean = false;
  isSaving: boolean = false;

  constructor(
    @Inject(MAT_BOTTOM_SHEET_DATA) public data: LessonData,
    private homeworkService: HomeworkService
  ) {
    this.loadHomework();
  }

  private loadHomework(): void {
    const dateStr = this.data.date.toISOString().split('T')[0]; // YYYY-MM-DD format
    this.isLoading = true;

    this.homeworkService.getHomework(dateStr, this.data.order).subscribe({
      next: (response) => {
        this.homework = response.homework || '';
        this.isLoading = false;
        console.log('Homework loaded from backend:', response);
      },
      error: (error) => {
        console.error('Error loading homework from backend:', error);
        // Fallback to localStorage if backend is not available
        const key = `homework_${dateStr}_${this.data.order}`;
        const saved = localStorage.getItem(key);
        this.homework = saved || '';
        this.isLoading = false;
      }
    });
  }

  saveHomework(): void {
    const dateStr = this.data.date.toISOString().split('T')[0]; // YYYY-MM-DD format
    this.isSaving = true;

    this.homeworkService.saveHomework(dateStr, this.data.order, this.homework).subscribe({
      next: (response) => {
        console.log('Homework saved to backend:', response);
        this.isSaving = false;
        // Also save to localStorage as backup
        const key = `homework_${dateStr}_${this.data.order}`;
        localStorage.setItem(key, this.homework);
      },
      error: (error) => {
        console.error('Error saving homework to backend:', error);
        // Fallback to localStorage if backend is not available
        const key = `homework_${dateStr}_${this.data.order}`;
        localStorage.setItem(key, this.homework);
        this.isSaving = false;
      }
    });
  }
}
