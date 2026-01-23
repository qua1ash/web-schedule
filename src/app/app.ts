import { Component, signal, ElementRef, ViewChild, AfterViewInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatBottomSheet } from '@angular/material/bottom-sheet';
import { MatDialog } from '@angular/material/dialog';
import { LessonDetailComponent } from './lesson-detail/lesson-detail';
import { CalendarComponent } from './calendar/calendar';
import { LoginDialog } from './login-dialog/login-dialog';
import { LessonsService, Lesson } from './services/lessons.service';

@Component({
  selector: 'app-root',
  imports: [CommonModule, CalendarComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})

export class App implements AfterViewInit {
  @ViewChild('weekButtons', { static: true }) weekButtons!: ElementRef;
  @ViewChild('lessonsContainer', { static: true }) lessonsContainer!: ElementRef;

  private touchStartX = 0;
  private touchStartY = 0;
  private touchEndX = 0;
  private touchEndY = 0;

  protected readonly title = signal('shedule');

  public lessons = signal<Lesson[]>([]);
  public isLoading = signal(false);
  public error = signal<string | null>(null);

  // Current week offset from today (0 = current week, -1 = previous week, 1 = next week)
  currentWeekOffset = signal(0);

  // Generate days for the current week
  days = signal(this.generateWeekDays(this.currentWeekOffset()));

  // Current date display
  currentDate = signal(this.getCurrentDateString());

  // Calendar visibility
  showCalendar = signal(false);

  // Selected day for highlighting in week buttons
  selectedDay = signal<Date>(new Date());

  // User authentication
  isLoggedIn = signal(false);
  isAdmin = signal(false);
  isEditor = signal(false);

  constructor(private bottomSheet: MatBottomSheet, private dialog: MatDialog, private lessonsService: LessonsService) {
    this.loadPersistedState();
  }

  ngAfterViewInit() {
    this.setupSwipeListeners();
  }

  private loadPersistedState(): void {
    // Always start with today's date on reload
    this.selectedDay.set(new Date());

    // Load week offset (but reset to 0 if it's from a very old date)
    const savedWeekOffset = localStorage.getItem('currentWeekOffset');
    if (savedWeekOffset) {
      const offset = parseInt(savedWeekOffset, 10);
      if (!isNaN(offset) && Math.abs(offset) <= 52) { // Allow reasonable week offsets
        this.currentWeekOffset.set(offset);
        this.days.set(this.generateWeekDays(offset));
        this.updateCurrentDate();
      } else {
        // Reset to current week if offset is too large
        this.currentWeekOffset.set(0);
        this.days.set(this.generateWeekDays(0));
        this.updateCurrentDate();
      }
    }

    // Load lessons for today's date
    this.loadLessonsForDate(this.selectedDay());
  }

  private savePersistedState(): void {
    localStorage.setItem('selectedDay', this.selectedDay().toISOString());
    localStorage.setItem('currentWeekOffset', this.currentWeekOffset().toString());
  }

  private setupSwipeListeners() {
    // Week buttons swipe for weeks
    const weekButtonsElement = this.weekButtons.nativeElement;
    weekButtonsElement.addEventListener('touchstart', (e: TouchEvent) => {
      this.touchStartX = e.changedTouches[0].screenX;
      this.touchStartY = e.changedTouches[0].screenY;
    }, { passive: true });

    weekButtonsElement.addEventListener('touchend', (e: TouchEvent) => {
      this.touchEndX = e.changedTouches[0].screenX;
      this.touchEndY = e.changedTouches[0].screenY;
      this.handleWeekSwipe();
    }, { passive: true });

    // Lessons container swipe for days
    const lessonsElement = this.lessonsContainer.nativeElement;
    lessonsElement.addEventListener('touchstart', (e: TouchEvent) => {
      this.touchStartX = e.changedTouches[0].screenX;
      this.touchStartY = e.changedTouches[0].screenY;
    }, { passive: true });

    lessonsElement.addEventListener('touchend', (e: TouchEvent) => {
      this.touchEndX = e.changedTouches[0].screenX;
      this.touchEndY = e.changedTouches[0].screenY;
      this.handleDaySwipe();
    }, { passive: true });
  }

  private handleWeekSwipe() {
    const deltaX = this.touchEndX - this.touchStartX;
    const deltaY = this.touchEndY - this.touchStartY;
    const minSwipeDistance = 50;

    // Check if horizontal swipe is greater than vertical
    if (Math.abs(deltaX) > Math.abs(deltaY) && Math.abs(deltaX) > minSwipeDistance) {
      if (deltaX > 0) {
        // Swipe right - previous week
        console.log('Week buttons: Swipe right detected - previous week');
        this.previousWeek();
      } else {
        // Swipe left - next week
        console.log('Week buttons: Swipe left detected - next week');
        this.nextWeek();
      }
    }
  }

  private handleDaySwipe() {
    const deltaX = this.touchEndX - this.touchStartX;
    const deltaY = this.touchEndY - this.touchStartY;
    const minSwipeDistance = 50;

    // Check if horizontal swipe is greater than vertical
    if (Math.abs(deltaX) > Math.abs(deltaY) && Math.abs(deltaX) > minSwipeDistance) {
      if (deltaX > 0) {
        // Swipe right - previous day
        console.log('Lessons container: Swipe right detected - previous day');
        this.selectPreviousDay();
      } else {
        // Swipe left - next day
        console.log('Lessons container: Swipe left detected - next day');
        this.selectNextDay();
      }
    }
  }

  private generateWeekDays(offset: number = 0): any[] {
    const today = new Date();
    const monday = new Date(today);
    monday.setDate(today.getDate() - today.getDay() + 1 + (offset * 7));

    const days = [];
    const dayNames = ['ПН', 'ВТ', 'СР', 'ЧТ', 'ПТ', 'СБ', 'ВС'];

    for (let i = 0; i < 7; i++) {
      const date = new Date(monday);
      date.setDate(monday.getDate() + i);
      days.push({
        date: date.getDate(),
        dayName: dayNames[i],
        fullDate: date
      });
    }
    return days;
  }

  private getCurrentDateString(): string {
    const today = new Date();
    const monthNames = ['Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь', 'Июль', 'Август', 'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь'];
    return monthNames[today.getMonth()];
  }

  nextWeek() {
    this.currentWeekOffset.update(offset => offset + 1);
    this.days.set(this.generateWeekDays(this.currentWeekOffset()));
    this.updateCurrentDate();
    this.savePersistedState();
  }

  previousWeek() {
    this.currentWeekOffset.update(offset => offset - 1);
    this.days.set(this.generateWeekDays(this.currentWeekOffset()));
    this.updateCurrentDate();
    this.savePersistedState();
  }

  private updateCurrentDate() {
    const today = new Date();
    today.setDate(today.getDate() + (this.currentWeekOffset() * 7));
    const monthNames = ['Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь', 'Июль', 'Август', 'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь'];
    this.currentDate.set(monthNames[today.getMonth()]);
  }

  openLessonDetail(lesson: any): void {
    const bottomSheetRef = this.bottomSheet.open(LessonDetailComponent, {
      data: { ...lesson, isAdmin: this.isAdmin(), isEditor: this.isEditor() },
      panelClass: 'lesson-bottom-sheet'
    });
    bottomSheetRef.afterDismissed().subscribe(() => {
      this.loadLessonsForDate(this.selectedDay());
    });
  }

  openCalendar(): void {
    this.showCalendar.set(!this.showCalendar());
  }

  openProfile(): void {
    const dialogRef = this.dialog.open(LoginDialog, {
      width: '400px',
      panelClass: 'login-dialog-panel',
      data: {}
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result && result.success) {
        this.isLoggedIn.set(true);
        this.isAdmin.set(result.role === 'admin');
        this.isEditor.set(result.role === 'editor');
        console.log('Login successful:', result);
      } else if (result && !result.success) {
        console.log('Login failed:', result.message);
        // Можно добавить уведомление об ошибке
      }
    });
  }

  onDaySelected(selectedDate: Date): void {
    // Calculate week offset for the selected date
    const today = new Date();
    const todayMonday = new Date(today);
    todayMonday.setDate(today.getDate() - today.getDay() + 1); // Monday of current week

    const selectedMonday = new Date(selectedDate);
    selectedMonday.setDate(selectedDate.getDate() - selectedDate.getDay() + 1); // Monday of selected week

    const diffTime = selectedMonday.getTime() - todayMonday.getTime();
    const diffDays = diffTime / (1000 * 60 * 60 * 24);
    const weekOffset = Math.round(diffDays / 7);

    this.currentWeekOffset.set(weekOffset);
    this.days.set(this.generateWeekDays(this.currentWeekOffset()));
    this.updateCurrentDate();
    this.selectedDay.set(selectedDate);
    this.showCalendar.set(false);

    // Load lessons for the selected date (for now, using mock data)
    this.loadLessonsForDate(selectedDate);

    this.savePersistedState();
  }

  isDaySelected(dayDate: Date): boolean {
    return dayDate.toDateString() === this.selectedDay().toDateString();
  }

  selectDayFromWeek(dayDate: Date): void {
    this.selectedDay.set(dayDate);
    this.loadLessonsForDate(dayDate);
    this.savePersistedState();
  }

  selectNextDay(): void {
    console.log('Swipe right detected - selecting next day');
    const currentDays = this.days();
    const currentIndex = currentDays.findIndex(day => this.isDaySelected(day.fullDate));
    const nextIndex = (currentIndex + 1) % currentDays.length;
    this.selectDayFromWeek(currentDays[nextIndex].fullDate);
  }

  selectPreviousDay(): void {
    console.log('Swipe left detected - selecting previous day');
    const currentDays = this.days();
    const currentIndex = currentDays.findIndex(day => this.isDaySelected(day.fullDate));
    const prevIndex = currentIndex === 0 ? currentDays.length - 1 : currentIndex - 1;
    this.selectDayFromWeek(currentDays[prevIndex].fullDate);
  }

  private loadLessonsForDate(date: Date): void {
    this.isLoading.set(true);
    this.error.set(null);
    this.lessonsService.getLessonsForDate(date).subscribe({
      next: (lessons) => {
        this.lessons.set(lessons);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Error loading lessons:', err);
        this.error.set('Failed to load lessons. Please try again.');
        this.lessons.set([]);
        this.isLoading.set(false);
      }
    });
  }
}

export class ScheduleComponent {
  
}
