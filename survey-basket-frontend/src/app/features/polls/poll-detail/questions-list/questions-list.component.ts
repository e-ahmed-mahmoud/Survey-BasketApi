import { Component, inject, signal, OnInit, input, Signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialogModule } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';
import { QuestionsService } from '../../../../core/services/questions.service';
import { Question, PaginationFilter } from '../../../../core/models';
import { AuthService } from '../../../../core/services/auth.service';

@Component({
    selector: 'app-questions-list',
    standalone: true,
    imports: [
        CommonModule,
        MatTableModule, MatPaginatorModule, MatSortModule,
        MatButtonModule, MatIconModule, MatProgressSpinnerModule, MatDialogModule, MatTooltipModule,
    ],
    templateUrl: './questions-list.component.html',
    styleUrls: ['./questions-list.component.scss'],
})
export class QuestionsListComponent implements OnInit {
    private readonly questionsService = inject(QuestionsService);
    private readonly snackBar = inject(MatSnackBar);
    private readonly authService = inject(AuthService);

    pollId = input.required<number>();

    questions = signal<Question[]>([]);
    loading = signal(true);
    isAdmin = signal<boolean>(this.authService.hasRole('Admin'));
    pageNumber = signal(1);
    pageSize = signal(10);
    totalCount = signal(0);

    displayedColumns = ['content', 'answers', 'actions'];

    ngOnInit(): void {
        this.loadQuestions();
    }

    private loadQuestions(): void {
        this.loading.set(true);
        const filter: PaginationFilter = {
            pageNumber: this.pageNumber(),
            pageSize: this.pageSize(),
        };

        this.questionsService.getAll(this.pollId(), filter).subscribe({
            next: (data) => {
                console.log('Loaded questions:', data.items);
                console.log(data.items);
                this.questions.set(data.items);
                this.totalCount.set(data.totalCount);
                this.loading.set(false);
                console.log(this.questions());
            },
            error: () => {
                this.loading.set(false);
                this.snackBar.open('Failed to load questions', 'OK', { panelClass: ['snack-error'] });
            },
        });
        console.log(this.questions());
    }

    onPageChange(event: PageEvent): void {
        this.pageNumber.set(event.pageIndex + 1);
        this.pageSize.set(event.pageSize);
        this.loadQuestions();
    }

    deleteQuestion(questionId: number): void {
        if (!confirm('Are you sure you want to delete this question?')) return;

        this.questionsService.delete(this.pollId(), questionId).subscribe({
            next: () => {
                this.snackBar.open('Question deleted successfully', 'OK', { panelClass: ['snack-success'] });
                this.loadQuestions();
            },
            error: () => {
                this.snackBar.open('Failed to delete question', 'OK', { panelClass: ['snack-error'] });
            },
        });
    }

    getTotalAnswers(question: Question): number {
        return question.answers?.length ?? 0;
    }
}
