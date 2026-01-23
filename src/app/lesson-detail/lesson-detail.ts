import { Component, Inject } from '@angular/core';
import { MAT_BOTTOM_SHEET_DATA, MatBottomSheetRef } from '@angular/material/bottom-sheet';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { HomeworkService } from '../services/homework.service';
import { LessonsService, LessonDetail } from '../services/lessons.service';

export interface LessonData {
  order: number;
  date: Date;
  start: string;
  end: string;
  title: string;
  teacher: string;
  room: string;
  isAdmin?: boolean;
  isEditor?: boolean;
}

@Component({
  selector: 'app-lesson-detail',
  imports: [CommonModule, FormsModule, MatButtonModule],
  templateUrl: './lesson-detail.html',
  styleUrls: ['./lesson-detail.scss']
})
export class LessonDetailComponent {
  lessonDetail: LessonDetail | null = null;
  homework: string = '';
  isLoading: boolean = false;
  isSaving: boolean = false;
  isDeleting: boolean = false;

  constructor(
    @Inject(MAT_BOTTOM_SHEET_DATA) public data: LessonData,
    private bottomSheetRef: MatBottomSheetRef<LessonDetailComponent>,
    private homeworkService: HomeworkService,
    private lessonsService: LessonsService
  ) {
    this.loadLessonDetail();
  }

  private loadLessonDetail(): void {
    this.isLoading = true;

    this.lessonsService.getLessonDetail(this.data.date, this.data.order).subscribe({
      next: (detail) => {
        this.lessonDetail = detail;
        this.homework = detail.homework || '';
        this.isLoading = false;
        console.log('Lesson detail loaded from backend:', detail);
      },
      error: (error) => {
        console.error('Error loading lesson detail from backend:', error);
        // Fallback to injected data and localStorage homework
        this.lessonDetail = {
          order: this.data.order,
          date: this.data.date.toISOString().split('T')[0],
          start: this.data.start,
          end: this.data.end,
          title: this.data.title,
          teacher: this.data.teacher,
          room: this.data.room,
          homework: ''
        };
        const dateStr = this.data.date.toISOString().split('T')[0];
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

  toggleCancelLesson(): void {
    const isCancelling = !this.lessonDetail?.isCancelled;
    const message = isCancelling ? 'Вы уверены, что хотите отменить эту пару?' : 'Вы уверены, что хотите восстановить эту пару?';
    console.log('Toggle button clicked, isCancelling:', isCancelling);
    if (confirm(message)) {
      console.log('Confirmed, toggling lesson cancel');
      this.isDeleting = true;
      this.lessonsService.toggleCancelLesson(this.data.date, this.data.order).subscribe({
        next: (response) => {
          console.log('Lesson toggled:', response);
          this.isDeleting = false;
          // Close the bottom sheet
          this.bottomSheetRef.dismiss();
        },
        error: (error) => {
          console.error('Error toggling lesson:', error);
          this.isDeleting = false;
          alert('Ошибка при обработке пары');
        }
      });
    } else {
      console.log('Toggle cancelled');
    }
  }
}
