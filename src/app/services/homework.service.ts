import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';

export interface HomeworkResponse {
  date: string;
  order?: string;
  homework: string;
  success: boolean;
  message?: string;
}

@Injectable({
  providedIn: 'root'
})
export class HomeworkService {
  private apiUrl = 'http://localhost:3001/api/homework';

  constructor(private http: HttpClient) {}

  /**
   * Получить домашнее задание для указанной даты и урока
   * @param date Дата в формате YYYY-MM-DD
   * @param order Порядок урока
   */
  getHomework(date: string, order: number): Observable<HomeworkResponse> {
    const fullUrl = `${this.apiUrl}/${date}/${order}`;
    console.log('Calling API:', fullUrl);
    return this.http.get<HomeworkResponse>(fullUrl).pipe(
      map(response => {
        console.log('API response:', response);
        return response;
      })
    );
  }

  /**
   * Сохранить домашнее задание для указанной даты и урока
   * @param date Дата в формате YYYY-MM-DD
   * @param order Порядок урока
   * @param homework Текст домашнего задания
   */
  saveHomework(date: string, order: number, homework: string): Observable<HomeworkResponse> {
    const fullUrl = `${this.apiUrl}/${date}/${order}`;
    console.log('Calling API:', fullUrl);
    return this.http.post<HomeworkResponse>(fullUrl, {
      homework
    }).pipe(
      map(response => {
        console.log('API response:', response);
        return response;
      })
    );
  }
}