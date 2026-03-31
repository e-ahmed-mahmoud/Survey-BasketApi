import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Poll, PollRequest } from '../models';

@Injectable({ providedIn: 'root' })
export class PollsService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/api/Polls`;

  getPolls(): Observable<Poll[]> {
    return this.http.get<Poll[]>(`${this.base}/GetPolls`);
  }

  getCurrentPolls(): Observable<Poll[]> {
    return this.http.get<Poll[]>(`${this.base}/GetCurrentPolls`);
  }

  getPollById(id: number): Observable<Poll> {
    return this.http.get<Poll>(`${this.base}/GetById/${id}`);
  }

  createPoll(payload: PollRequest): Observable<Poll> {
    return this.http.post<Poll>(`${this.base}/AddPoll`, payload);
  }

  updatePoll(id: number, payload: PollRequest): Observable<void> {
    return this.http.put<void>(`${this.base}/UpdatePoll/${id}`, payload);
  }

  deletePoll(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/DeletePoll/${id}`);
  }

  togglePublished(id: number): Observable<void> {
    return this.http.put<void>(`${this.base}/TogglePublishedStatus/${id}/togglePublished`, {});
  }
}
