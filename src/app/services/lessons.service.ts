import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';

export interface Lesson {
  order: number;
  date: Date;
  start: string;
  end: string;
  title: string;
  teacher: string;
  room: string;
}

export interface LessonDetail {
  order: number;
  date: string;
  start: string;
  end: string;
  title: string;
  teacher: string;
  room: string;
  homework: string;
}

interface ApiLesson {
  order: number;
  date: string;
  start: string;
  end: string;
  title: string;
  teacher: string;
  room: string;
}

@Injectable({
  providedIn: 'root'
})
export class LessonsService {
  private apiUrl = 'http://localhost:3001/api/lessons';

  constructor(private http: HttpClient) {}

  getLessonsForDate(date: Date): Observable<Lesson[]> {
    const dateString = date.toISOString().split('T')[0]; // YYYY-MM-DD
    const fullUrl = `${this.apiUrl}/${dateString}`;
    console.log('Calling API:', fullUrl);
    return this.http.get<ApiLesson[]>(fullUrl).pipe(
      map(lessons => {
        console.log('API response:', lessons);
        return lessons.map(lesson => ({
          ...lesson,
          date: new Date(lesson.date) // Convert string to Date
        }));
      })
    );
  }

  getLessonDetail(date: Date, order: number): Observable<LessonDetail> {
    const dateString = date.toISOString().split('T')[0]; // YYYY-MM-DD
    const fullUrl = `${this.apiUrl}/${dateString}/${order}`;
    console.log('Calling API for lesson detail:', fullUrl);
    return this.http.get<LessonDetail>(fullUrl).pipe(
      map(detail => {
        console.log('Lesson detail API response:', detail);
        return detail;
      })
    );
  }
}