import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Question, QuestionRequest, PaginatedList, PaginationFilter } from '../models';

@Injectable({ providedIn: 'root' })
export class QuestionsService {
  private readonly http = inject(HttpClient);
  private readonly base = (pollId: number) =>
    `${environment.apiUrl}/api/Polls/${pollId}/Questions`;

  getAll(pollId: number, filter: PaginationFilter = {}): Observable<PaginatedList<Question>> {
    let params = new HttpParams();
    if (filter.pageNumber) params = params.set('PageNumber', filter.pageNumber);
    if (filter.pageSize) params = params.set('PageSize', filter.pageSize);
    if (filter.search) params = params.set('Search', filter.search);
    if (filter.sort) params = params.set('Sort', filter.sort);
    if (filter.sortDir) params = params.set('SortDir', filter.sortDir);
    return this.http.get<PaginatedList<Question>>(`${this.base(pollId)}/GetAll`, { params });
  }

  getById(pollId: number, questionId: number): Observable<Question> {
    return this.http.get<Question>(`${this.base(pollId)}/GetQuestionById/${questionId}`);
  }

  add(pollId: number, payload: QuestionRequest): Observable<Question> {
    return this.http.post<Question>(`${this.base(pollId)}/AddQuestion`, payload);
  }

  update(pollId: number, questionId: number, payload: QuestionRequest): Observable<void> {
    return this.http.put<void>(`${this.base(pollId)}/UpdateQuestion/${questionId}`, payload);
  }

  delete(pollId: number, questionId: number): Observable<void> {
    return this.http.delete<void>(`${this.base(pollId)}/DeleteQuestion/${questionId}`);
  }
}
