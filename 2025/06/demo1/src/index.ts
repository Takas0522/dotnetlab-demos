const button = document.getElementById("test");
if (button) {
  button.onclick = () => {
    const output = document.getElementById("output");
    if (output) {
        output.innerText = output.innerText + "Push!! |";
    }
  };
}