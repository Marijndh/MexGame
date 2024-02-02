import androidx.compose.foundation.layout.Column
import androidx.compose.material.Button
import androidx.compose.material.Text
import androidx.compose.runtime.Composable
import cafe.adriel.voyager.core.screen.Screen

class PlayerScreen : Screen {
    @Composable
    override fun Content() {
        Column(){
            Text("Kies Speler")
        }
    }

}