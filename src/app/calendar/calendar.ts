import { Component, signal, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-calendar',
  imports: [CommonModule],
  templateUrl: './calendar.html',
  styleUrl: './calendar.scss'
})
export class CalendarComponent {
  @Output() daySelected = new EventEmitter<Date>();

  currentMonth = signal(new Date());
  selectedDate = signal<Date | null>(null);

  getDaysInMonth(): Date[] {
    const year = this.currentMonth().getFullYear();
    const month = this.currentMonth().getMonth();
    const firstDay = new Date(year, month, 1);
    const lastDay = new Date(year, month + 1, 0);
    const days: Date[] = [];

    // Add empty cells for days before the first day of the month
    const startDay = firstDay.getDay() === 0 ? 6 : firstDay.getDay() - 1; // Monday first
    for (let i = 0; i < startDay; i++) {
      days.push(null as any);
    }

    // Add days of the month
    for (let day = 1; day <= lastDay.getDate(); day++) {
      days.push(new Date(year, month, day));
    }

    return days;
  }

  getMonthName(): string {
    const months = [
      'Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь',
      'Июль', 'Август', 'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь'
    ];
    return months[this.currentMonth().getMonth()];
  }

  getYear(): number {
    return this.currentMonth().getFullYear();
  }

  previousMonth(): void {
    this.currentMonth.update(month => {
      const newMonth = new Date(month);
      newMonth.setMonth(month.getMonth() - 1);
      return newMonth;
    });
  }

  nextMonth(): void {
    this.currentMonth.update(month => {
      const newMonth = new Date(month);
      newMonth.setMonth(month.getMonth() + 1);
      return newMonth;
    });
  }

  selectDay(day: Date): void {
    if (day) {
      this.selectedDate.set(day);
      this.daySelected.emit(day);
    }
  }

  isToday(day: Date): boolean {
    if (!day) return false;
    const today = new Date();
    return day.toDateString() === today.toDateString();
  }

  isSelected(day: Date): boolean {
    if (!day || !this.selectedDate()) return false;
    return day.toDateString() === this.selectedDate()!.toDateString();
  }
}