import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

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
  private readonly baseUrl = 'http://localhost:3001/api';

  constructor(private http: HttpClient) {}

  /**
   * Получить домашнее задание для указанной даты и урока
   * @param date Дата в формате YYYY-MM-DD
   * @param order Порядок урока
   */
  getHomework(date: string, order: number): Observable<HomeworkResponse> {
    return this.http.get<HomeworkResponse>(`${this.baseUrl}/homework/${date}/${order}`);
  }

  /**
   * Сохранить домашнее задание для указанной даты и урока
   * @param date Дата в формате YYYY-MM-DD
   * @param order Порядок урока
   * @param homework Текст домашнего задания
   */
  saveHomework(date: string, order: number, homework: string): Observable<HomeworkResponse> {
    return this.http.post<HomeworkResponse>(`${this.baseUrl}/homework/${date}/${order}`, {
      homework
    });
  }
}