using Pastel;

string[] logo = {
" ███████████ ████████████".Pastel(Color.Blue1),
" ███████████ ████████████".Pastel(Color.Blue1),
" ███████████ ████████████".Pastel(Color.Blue2),
" ███████████ ████████████".Pastel(Color.Blue2),
" ▄▄▄▄▄▄▄▄▄▄▄ ▄▄▄▄▄▄▄▄▄▄▄▄".Pastel(Color.Blue3),
" ███████████ ████████████".Pastel(Color.Blue3),
" ███████████ ████████████".Pastel(Color.Blue3),
" ███████████ ████████████".Pastel(Color.Blue4),
" ███████████ ████████████".Pastel(Color.Blue4),
" "
};

StatsGenerator statGenerator = new StatsGenerator();
await statGenerator.ProcessStats();
statGenerator.PrintStat(logo);


