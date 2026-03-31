import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, Validators, FormArray, FormGroup } from '@angular/forms';
import { MatRadioModule } from '@angular/material/radio';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { VotesService } from '../../../core/services/votes.service';
import { PollQuestion } from '../../../core/models';

@Component({
  selector: 'app-submit-vote',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule,
    MatRadioModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule,
  ],
  template: `
    <div class="vote-page fade-in">
      @if (loading()) {
        <div style="display:flex;justify-content:center;padding:80px"><mat-spinner diameter="48" /></div>
      } @else {
        <div class="page-header">
          <div class="header-left">
            <h1 class="page-title">Submit Your Vote</h1>
            <p class="page-subtitle">Answer all questions to submit</p>
          </div>
        </div>

        <form [formGroup]="form" (ngSubmit)="onSubmit()">
          @for (question of questions(); track question.id; let i = $index) {
            <div class="question-card sb-card">
              <h3 class="question-text">{{ i + 1 }}. {{ question.content }}</h3>
              <mat-radio-group [formControlName]="'q_' + question.id" class="answers-group">
                @for (answer of question.answers; track answer.id) {
                  <mat-radio-button [value]="answer.id" class="answer-option" [id]="'answer-' + answer.id">
                    {{ answer.content }}
                  </mat-radio-button>
                }
              </mat-radio-group>
            </div>
          }

          <div class="submit-row">
            <button mat-raised-button color="primary" type="submit" id="submit-vote-btn"
              [disabled]="submitting() || form.invalid">
              @if (submitting()) {
                <mat-spinner diameter="20" />
              } @else {
                <ng-container>
                  <mat-icon>how_to_vote</mat-icon> Submit Vote
                </ng-container>
              }
            </button>
          </div>
        </form>
      }
    </div>
  `,
  styles: [`
    .vote-page { max-width: 720px; margin: 0 auto; }
    .question-card { margin-bottom: 20px; }
    .question-text { font-size: 1.1rem; font-weight: 600; margin: 0 0 16px; }
    .answers-group { display: flex; flex-direction: column; gap: 10px; }
    .answer-option { ::ng-deep .mdc-form-field { width: 100%; padding: 8px 12px; border-radius: 8px; border: 1px solid var(--border-color); cursor: pointer; transition: background 0.15s; &:hover { background: var(--page-bg); } } }
    .submit-row { display: flex; justify-content: flex-end; margin-top: 8px; }
  `],
})
export class SubmitVoteComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly votesService = inject(VotesService);
  private readonly snackBar = inject(MatSnackBar);
  private readonly fb = inject(FormBuilder);

  questions = signal<PollQuestion[]>([]);
  loading = signal(true);
  submitting = signal(false);

  form: FormGroup = this.fb.group({});

  ngOnInit(): void {
    const pollId = Number(this.route.snapshot.paramMap.get('pollId'));
    this.votesService.getPollQuestions(pollId).subscribe({
      next: (questions) => {
        this.questions.set(questions);
        // Build dynamic form controls per question
        questions.forEach((q) => {
          this.form.addControl('q_' + q.id, this.fb.control(null, Validators.required));
        });
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  onSubmit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.submitting.set(true);
    const pollId = Number(this.route.snapshot.paramMap.get('pollId'));

    const voteAnswers = this.questions().map((q) => ({
      questionId: q.id,
      answerId: this.form.value['q_' + q.id],
    }));

    this.votesService.submitVote(pollId, { voteAnswers }).subscribe({
      next: () => {
        this.snackBar.open('Vote submitted successfully! Thank you.', 'OK', { panelClass: ['snack-success'] });
        this.router.navigate(['/votes']);
      },
      error: () => this.submitting.set(false),
    });
  }
}
